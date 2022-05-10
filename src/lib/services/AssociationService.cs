using Conventions.Models;
using Conventions.Storage;

namespace Conventions.Services
{
    public interface IAssociationService
    {
        Task AssociateVenue(string conventionId, string venueId);
        Task DisassociateVenue(string conventionId, string venueId);
        Task<IEnumerable<Venue?>> GetConventionAssociatedVenues(string conventionId);

        Task AssociateEvent(string conventionId, string eventId);
        Task DisassociateEvent(string conventionId, string eventId);
        Task<IEnumerable<Event?>> GetConventionAssociatedEvents(string conventionId);

        Task RegisterForEvent(string conventionId, string eventId, string userId);
        Task UnregisterForEvent(string conventionId, string eventId, string userId);
    }

    public class AssociationService : IAssociationService
    {
        private readonly IConventionService _conventionService;
        private readonly IVenueService _venueService;
        private readonly IEventService _eventService;
        private readonly IUserService _userService;
        private readonly IEventRegistrationStore _eventRegistrationStore;
        private readonly IConventionVenueAssociationStore _venueAssociationStore;
        private readonly IConventionEventAssociationStore _eventAssociationStore;

        public AssociationService(IConventionService conventionService,
                                  IVenueService venueService,
                                  IEventService eventService,
                                  IUserService userService,
                                  IEventRegistrationStore eventRegistrationStore,
                                  IConventionVenueAssociationStore venueAssociationStore,
                                  IConventionEventAssociationStore eventAssociationStore)
        {
            _conventionService = conventionService;
            _venueService = venueService;
            _eventService = eventService;
            _userService = userService;
            _eventRegistrationStore = eventRegistrationStore;
            _venueAssociationStore = venueAssociationStore;
            _eventAssociationStore = eventAssociationStore;
        }

        #region Venues
        public async Task AssociateVenue(string conventionId, string venueId)
        {
            var conventionLookTask = _conventionService.GetByIdAsync(conventionId);
            var venueLookupTask = _venueService.GetVenueAsync(venueId);

            var tasks = new Task[] { conventionLookTask, venueLookupTask };
            await Task.WhenAll(tasks);

            var convention = conventionLookTask.Result;
            if (convention is null)
            {
                throw new ArgumentException(nameof(conventionId));
            }

            var venue = venueLookupTask.Result;
            if (venue is null)
            {
                throw new ArgumentException(nameof(venueId));
            }

            // check if associated
            var association = await _venueAssociationStore.GetByConventionIdAndVenueIdAsync(conventionId, venueId);
            if (association is not null)
            {
                return;
            }

            await _venueAssociationStore.AddAsync(new ConventionVenueAssociation()
            {
                ConventionId = conventionId,
                VenueId = venueId,
            });
        }

        public async Task DisassociateVenue(string conventionId, string venueId)
        {
            var conventionLookTask = _conventionService.GetByIdAsync(conventionId);
            var venueLookupTask = _venueService.GetVenueAsync(venueId);

            var tasks = new Task[] { conventionLookTask, venueLookupTask };
            await Task.WhenAll(tasks);

            var convention = conventionLookTask.Result;
            if (convention is null)
            {
                throw new ArgumentException(nameof(conventionId));
            }

            var venue = venueLookupTask.Result;
            if (venue is null)
            {
                throw new ArgumentException(nameof(venueId));
            }

            // check if associated
            var association = await _venueAssociationStore.GetByConventionIdAndVenueIdAsync(conventionId, venueId);
            if (association is null)
            {
                return;
            }

            await _venueAssociationStore.DeleteAsync(association);
        }

        public async Task<IEnumerable<Venue?>> GetConventionAssociatedVenues(string conventionId)
        {
            var associations = await _venueAssociationStore.GetByConventionIdAsync(conventionId);
            var venueTasks = associations.Select(x => _venueService.GetVenueAsync(x.VenueId)).ToList();

            await Task.WhenAll(venueTasks);

            return venueTasks.Where(x => x.Result != null)
                             .Select(x => x.Result)
                             .ToList();
        }

        #endregion
        #region Events
        public async Task AssociateEvent(string conventionId, string eventId)
        {
            var conventionLookTask = _conventionService.GetByIdAsync(conventionId);
            var venueLookupTask = _eventService.GetByIdAsync(eventId);

            var tasks = new Task[] { conventionLookTask, venueLookupTask };
            await Task.WhenAll(tasks);

            var convention = conventionLookTask.Result;
            if (convention is null)
            {
                throw new ArgumentException(nameof(conventionId));
            }

            var venue = venueLookupTask.Result;
            if (venue is null)
            {
                throw new ArgumentException(nameof(eventId));
            }

            // check if associated
            var association = await _eventAssociationStore.GetByConventionIdAndEventIdAsync(conventionId, eventId);
            if (association is not null)
            {
                return;
            }

            _ = await _eventAssociationStore.AddAsync(new ConventionEventAssociation()
            {
                ConventionId = conventionId,
                EventId = eventId,
            });
        }

        public async Task DisassociateEvent(string conventionId, string eventId)
        {
            var conventionLookTask = _conventionService.GetByIdAsync(conventionId);
            var venueLookupTask = _eventService.GetByIdAsync(eventId);

            var tasks = new Task[] { conventionLookTask, venueLookupTask };
            await Task.WhenAll(tasks);

            var convention = conventionLookTask.Result;
            if (convention is null)
            {
                throw new ArgumentException(nameof(conventionId));
            }

            var venue = venueLookupTask.Result;
            if (venue is null)
            {
                throw new ArgumentException(nameof(eventId));
            }

            // check if associated
            var association = await _eventAssociationStore.GetByConventionIdAndEventIdAsync(conventionId, eventId);
            if (association is null)
            {
                return;
            }

            await _eventAssociationStore.DeleteAsync(association);
        }

        public async Task<IEnumerable<Event?>> GetConventionAssociatedEvents(string conventionId)
        {
            var associations = await _eventAssociationStore.GetByConventionIdAsync(conventionId);
            var eventTasks = associations.Select(x => _eventService.GetByIdAsync(x.EventId)).ToList();

            await Task.WhenAll(eventTasks);

            return eventTasks.Where(x => x.Result != null)
                             .Select(x => x.Result)
                             .ToList();
        }

        #endregion

        #region Event Reservations

        public async Task RegisterForEvent(string conventionId, string eventId, string userId)
        {
            var conventionLookTask = _conventionService.GetByIdAsync(conventionId);
            var eventLookupTask = _eventService.GetByIdAsync(eventId);
            var userLookupTask = _userService.GetByIdAsync(userId);

            var tasks = new Task[] { conventionLookTask, eventLookupTask, userLookupTask };
            await Task.WhenAll(tasks);

            var convention = conventionLookTask.Result;
            if (convention is null)
            {
                throw new ArgumentException(nameof(conventionId));
            }

            var e = eventLookupTask.Result;
            if (e is null)
            {
                throw new ArgumentException(nameof(eventId));
            }

            var user = userLookupTask.Result;
            if (user is null)
            {
                throw new ArgumentException(nameof(userId));
            }

            // check if convention and event are associated
            var conventionEventAssoication = await _eventAssociationStore.GetByConventionIdAndEventIdAsync(conventionId, eventId);
            if (conventionEventAssoication is null)
            {
                throw new ArgumentOutOfRangeException(nameof(conventionId), $"{conventionId} not associated with {eventId}");
            }

            // check if user already has reservation for event
            var userEventAssociation = await _eventRegistrationStore.GetByEventIdAndUserIdAsync(eventId, userId);
            if (userEventAssociation is not null)
            {
                return;
            }

            _ = await _eventRegistrationStore.AddAsync(new EventReservation()
            {
                EventId = eventId,
                UserId = userId,
            });
        }

        public async Task UnregisterForEvent(string conventionId, string eventId, string userId)
        {
            var conventionLookTask = _conventionService.GetByIdAsync(conventionId);
            var eventLookupTask = _eventService.GetByIdAsync(eventId);
            var userLookupTask = _userService.GetByIdAsync(userId);

            var tasks = new Task[] { conventionLookTask, eventLookupTask, userLookupTask };
            await Task.WhenAll(tasks);

            var convention = conventionLookTask.Result;
            if (convention is null)
            {
                throw new ArgumentException(nameof(conventionId));
            }

            var e = eventLookupTask.Result;
            if (e is null)
            {
                throw new ArgumentException(nameof(eventId));
            }

            var user = userLookupTask.Result;
            if (user is null)
            {
                throw new ArgumentException(nameof(userId));
            }

            // check if convention and event are associated
            var conventionEventAssoication = await _eventAssociationStore.GetByConventionIdAndEventIdAsync(conventionId, eventId);
            if (conventionEventAssoication is null)
            {
                throw new ArgumentOutOfRangeException(nameof(conventionId), $"{conventionId} not associated with {eventId}");
            }

            // check if user already has reservation for event
            var userEventAssociation = await _eventRegistrationStore.GetByEventIdAndUserIdAsync(eventId, userId);
            if (userEventAssociation is null)
            {
                return;
            }

            await _eventRegistrationStore.DeleteAsync(userEventAssociation);
        }
        #endregion
    }
}