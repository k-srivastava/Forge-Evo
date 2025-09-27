using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace ForgeEvo.Core.Engine;

/// <summary>
///     Represents a thread-safe event system that allows subscribing, unsubscribing, and posting notifications to all
///     subscribers.
/// </summary>
public sealed class Event
{
    /// <summary>
    ///     Serves as a synchronization lock to ensure thread-safe access to the collection of subscribers for the event.
    /// </summary>
    private readonly Lock _gate = new();

    /// <summary>
    ///     Unique ID of the event.
    /// </summary>
    public readonly int Id;

    /// <summary>
    ///     Name of the event.
    /// </summary>
    public readonly string Name;

    /// <summary>
    ///     Maintains a thread-safe array of actions that are subscribed to an event. Actions within this array are executed
    ///     when the event is posted.
    /// </summary>
    private volatile Action[] _subscribers = [];

    /// <summary>
    ///     Create a new event.
    /// </summary>
    /// <param name="id">Unique ID of the event.</param>
    /// <param name="name">Name of the event, follows the convention: <c>"&lt;Event-Name&gt;"</c>.</param>
    internal Event(int id, string name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    ///     Add a subscriber to an event.
    /// </summary>
    /// <param name="event">The event to which the subscriber will be added.</param>
    /// <param name="subscriber">The subscriber to add to the event.</param>
    /// <returns>The updated event instance with the new subscriber added.</returns>
    public static Event operator +(Event @event, Action subscriber)
    {
        @event.Subscribe(subscriber);
        return @event;
    }

    /// <summary>
    ///     Remove a subscriber from an event.
    /// </summary>
    /// <param name="event">The event from which the subscriber will be removed.</param>
    /// <param name="subscriber">The subscriber to remove from the event.</param>
    /// <returns>The updated event instance with the subscriber removed.</returns>
    public static Event operator -(Event @event, Action subscriber)
    {
        @event.Unsubscribe(subscriber);
        return @event;
    }

    /// <summary>
    ///     Add a subscriber to an event.
    /// </summary>
    /// <param name="subscriber">The subscriber to add to the event.</param>
    public void Subscribe(Action subscriber)
    {
        lock (_gate)
        {
            Action[] current = _subscribers;
            if (Array.IndexOf(current, subscriber) >= 0)
                return;

            var updated = new Action[current.Length + 1];
            Array.Copy(current, updated, current.Length);
            updated[current.Length] = subscriber;

            _subscribers = updated;
        }
    }

    /// <summary>
    ///     Remove a subscriber from an event.
    /// </summary>
    /// <param name="subscriber">The subscriber to remove from the event.</param>
    public void Unsubscribe(Action subscriber)
    {
        lock (_gate)
        {
            Action[] current = _subscribers;
            int index = Array.IndexOf(current, subscriber);

            if (index < 0)
                return;

            var updated = new Action[current.Length - 1];

            Array.Copy(current, updated, index);
            Array.Copy(current, index + 1, updated, index, current.Length - index - 1);

            _subscribers = updated;
        }
    }

    /// <summary>
    ///     Posts a notification to all subscribers of the event. Each subscriber's action is executed in the order they were
    ///     added. Exceptions thrown by individual subscribers are caught and logged without interrupting the execution of
    ///     other subscribers.
    /// </summary>
    public void Post()
    {
        // ReSharper disable InconsistentlySynchronizedField
        // This is a volatile read and is intentionally lock-free.
        Action[] snapshot = _subscribers;
        // ReSharper restore InconsistentlySynchronizedField

        foreach (Action subscriber in snapshot)
        {
            try
            {
                subscriber();
            }

            catch (Exception e)
            {
                Console.WriteLine(
                    $"[Event: {Name}] Handler '{subscriber.Method.Name}' threw an exception: {e.Message}"
                );
            }
        }
    }

    public override string ToString() => $"Event(Name={Name}, Id={Id}, Subscribers={Count()})";

    /// <summary>
    ///     Retrieves the current number of subscribers for the event.
    /// </summary>
    /// <returns>The total count of subscribers currently subscribed to the event.</returns>
    private int Count()
    {
        lock (_gate) return _subscribers.Length;
    }
}

/// <summary>
///     Provides a centralized, thread-safe system to manage the creation, retrieval, and deletion of named and internal
///     events.
/// </summary>
/// <remarks>
///     The <see cref="EventBus" /> class facilitates event registration, enabling the association of names or IDs with
///     events. It offers methods for retrieving events by name or ID, handling both user-defined and internal events, and
///     managing their lifecycles.
/// </remarks>
public static class EventBus
{
    /// <summary>
    ///     A thread-safe dictionary that maps unique event IDs to their corresponding <see cref="Event" /> instances.
    /// </summary>
    /// <remarks>
    ///     Used internally by the <see cref="EventBus" /> to store and manage events based on their numeric IDs.
    /// </remarks>
    private static readonly ConcurrentDictionary<int, Event> EventsById = [];

    /// <summary>
    ///     A thread-safe dictionary that maps event names to their corresponding unique integer IDs.
    /// </summary>
    /// <remarks>
    ///     Used internally by the <see cref="EventBus" /> to ensure quick and consistent resolution of event names to their
    ///     unique IDs.
    /// </remarks>
    private static readonly ConcurrentDictionary<string, int> IdByName = [];

    /// <summary>
    ///     Represents a collection of event names that are flagged as internal.
    /// </summary>
    /// <remarks>
    ///     Internal event names are used to identify events that are considered part of the internal workings of the system.
    ///     These events cannot be deleted using their names to ensure system stability and integrity.
    /// </remarks>
    private static readonly HashSet<string> InternalNames = new(StringComparer.Ordinal);

    /// <summary>
    ///     Serves as a synchronization lock to ensure thread-safe access to the collection of events and their related
    ///     operations.
    /// </summary>
    private static readonly Lock Gate = new();

    /// <summary>
    ///     Tracks the next unique identifier for events, ensuring sequential and thread-safe ID generation.
    /// </summary>
    private static int _nextId;

    /// <summary>
    ///     Generates the next unique identifier for events in a thread-safe manner.
    /// </summary>
    /// <returns>The next unique integer identifier.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int NextId() => Interlocked.Increment(ref _nextId);

    /// <summary>
    ///     Creates a new event with the specified name and registers it on the event bus.
    /// </summary>
    /// <param name="name">The name of the event to create. Must be unique and non-empty.</param>
    /// <param name="internal">Indicates whether the event is an internal event.</param>
    /// <returns>Returns the newly created <see cref="Event" /> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided name is null, empty, or consists only of whitespace.</exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when an event with the specified name already exists, or if there is
    ///     a conflict while registering the event.
    /// </exception>
    public static Event Create(string name, bool @internal = false)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Event name cannot be null or whitespace.", nameof(name));

        lock (Gate)
        {
            if (IdByName.ContainsKey(name))
                throw new InvalidOperationException($"Event with name '{name}' already exists.");

            int id = NextId();
            var @event = new Event(id, name);

            if (!EventsById.TryAdd(id, @event))
                throw new InvalidOperationException($"Event with id {id} already exists.");

            if (!IdByName.TryAdd(name, id))
            {
                EventsById.TryRemove(id, out _);
                throw new InvalidOperationException($"Event with name '{name}' already exists.");
            }

            if (@internal) InternalNames.Add(name);

            return @event;
        }
    }

    /// <summary>
    ///     Attempts to retrieve an event by its name.
    /// </summary>
    /// <param name="name">The name of the event to retrieve.</param>
    /// <param name="event">The event associated with the given name, or <c>null</c> if no such event exists.</param>
    /// <returns>True if the event was found, otherwise false.</returns>
    public static bool TryGetByName(string name, out Event? @event)
    {
        @event = null;
        return IdByName.TryGetValue(name, out int id) && EventsById.TryGetValue(id, out @event);
    }

    /// <summary>
    ///     Retrieves an event by its unique name.
    /// </summary>
    /// <param name="name">The unique name of the event to retrieve.</param>
    /// <returns>The <see cref="Event" /> associated with the specified name.</returns>
    /// <exception cref="InvalidOperationException">Thrown if an event with the specified name does not exist.</exception>
    public static Event GetByName(string name) => !TryGetByName(name, out Event? @event)
        ? throw new InvalidOperationException($"Event with name '{name}' does not exist.")
        : @event!;

    /// <summary>
    ///     Attempts to retrieve an event by its unique ID.
    /// </summary>
    /// <param name="id">The unique identifier of the event.</param>
    /// <param name="event">The event associated with the given ID, or <c>null</c> if no such event exists.</param>
    /// <returns><c>true</c> if an event with the specified ID is found; otherwise, <c>false</c>.</returns>
    public static bool TryGetById(int id, out Event? @event) => EventsById.TryGetValue(id, out @event);

    /// <summary>
    ///     Retrieves an event by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the event.</param>
    /// <returns>The <see cref="Event" /> associated with the given ID.</returns>
    /// <exception cref="InvalidOperationException">Thrown if an event with the specified ID does not exist.</exception>
    public static Event GetById(int id) => !TryGetById(id, out Event? @event)
        ? throw new InvalidOperationException($"Event with id {id} does not exist.")
        : @event!;

    /// <summary>
    ///     Deletes an existing event by its name.
    /// </summary>
    /// <param name="name">The unique name of the event to delete.</param>
    /// <exception cref="KeyNotFoundException">Thrown if no event with the specified name exists.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the specified event is internal and cannot be deleted.</exception>
    public static void DeleteByName(string name)
    {
        lock (Gate)
        {
            if (!IdByName.TryGetValue(name, out int id))
                throw new KeyNotFoundException($"Event with name '{name}' does not exist.");

            if (InternalNames.Contains(name))
                throw new InvalidOperationException($"Event with name '{name}' is internal and cannot be deleted.");

            IdByName.TryRemove(name, out _);
            EventsById.TryRemove(id, out _);
        }
    }

    /// <summary>
    ///     Deletes an event by its unique ID.
    /// </summary>
    /// <param name="id">The unique identifier of the event to be deleted.</param>
    /// <exception cref="KeyNotFoundException">
    ///     Thrown when an event with the specified ID does not exist.
    /// </exception>
    /// <remarks>
    ///     Unlike <see cref="DeleteByName" />, this method allows deletion of internal events. Internal events can only be
    ///     deleted using their ID and not their name.
    /// </remarks>
    public static void DeleteById(int id)
    {
        lock (Gate)
        {
            if (!EventsById.TryRemove(id, out Event? @event))
                throw new KeyNotFoundException($"Event with id {id} does not exist.");

            IdByName.TryRemove(@event.Name, out _);
            InternalNames.Remove(@event.Name);
        }
    }

    /// <summary>
    ///     Registers one or more internal events in the system.
    /// </summary>
    /// <param name="events">An array of <see cref="InternalEvent" /> to be registered as internal events.</param>
    public static void RegisterInternalEvents(params InternalEvent[] events)
    {
        foreach (InternalEvent @event in events)
        {
            string name = @event.ToName();

            lock (Gate)
            {
                if (!IdByName.ContainsKey(name))
                    Create(name, true);
            }
        }
    }

    /// <summary>
    ///     Retrieves a read-only collection of all registered internal event names.
    /// </summary>
    /// <returns>A read-only collection of strings representing the names of registered internal events.</returns>
    public static IReadOnlyCollection<string> ListInternalEvents()
    {
        lock (Gate) return InternalNames.ToArray();
    }
}

/// <summary>
///     Represents predefined internal events that can be registered and used within the eventing system.
/// </summary>
public enum InternalEvent
{
    /// <summary>
    ///     Event fired once when the <see cref="Game" /> has completed initialization.
    /// </summary>
    GameInitialized,

    /// <summary>
    ///     Event fired every time the <see cref="Game" /> is updated in the main game loop.
    /// </summary>
    GameUpdated,

    /// <summary>
    ///     Event fired every time the <see cref="Game" /> is rendered in the main game loop.
    /// </summary>
    GameRendered,

    /// <summary>
    ///     Event fired from the <see cref="InputHandler" /> when any key is pressed.
    /// </summary>
    KeyPressed,

    /// <summary>
    ///     Event fired from the <see cref="InputHandler" /> when any key is released.
    /// </summary>
    KeyReleased,

    /// <summary>
    ///     Event fired from the <see cref="InputHandler" /> when any mouse button is pressed.
    /// </summary>
    MousePressed,

    /// <summary>
    ///     Event fired from the <see cref="InputHandler" /> when any mouse button is released.
    /// </summary>
    MouseReleased
}

/// <summary>
///     Provides extension methods for the <see cref="InternalEvent" /> enumeration, enabling additional functionality,
///     such as getting the string representation of predefined internal events.
/// </summary>
public static class InternalEventExtensions
{
    /// <summary>
    ///     Converts the specified <see cref="InternalEvent" /> enumeration value to its corresponding string representation.
    /// </summary>
    /// <param name="event">The <see cref="InternalEvent" /> to convert.</param>
    /// <returns>The string representation of the specified <see cref="InternalEvent" />.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when the provided <see cref="InternalEvent" /> value does not match a defined case.
    /// </exception>
    public static string ToName(this InternalEvent @event) => @event switch
    {
        InternalEvent.GameInitialized => "<Game-Initialized>",
        InternalEvent.GameUpdated => "<Game-Updated>",
        InternalEvent.GameRendered => "<Game-Rendered>",
        InternalEvent.KeyPressed => "<Key-Pressed>",
        InternalEvent.KeyReleased => "<Key-Released>",
        InternalEvent.MousePressed => "<Mouse-Pressed>",
        InternalEvent.MouseReleased => "<Mouse-Released>",

        _ => throw new ArgumentOutOfRangeException(nameof(@event), @event, null)
    };
}