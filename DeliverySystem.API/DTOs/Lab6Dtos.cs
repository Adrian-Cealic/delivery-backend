namespace DeliverySystem.API.DTOs;

public record DeliveryQuoteRequest(decimal DistanceKm, decimal WeightKg, string Strategy);
public record DeliveryQuoteResponse(string Strategy, decimal Cost, double EtaMinutes);

public record DraftLineDto(string ProductName, int Quantity, decimal UnitPrice, decimal Weight);
public record AddDraftLineRequest(string ProductName, int Quantity, decimal UnitPrice, decimal Weight);
public record SaveDraftRequest(string Label);
public record RestoreDraftRequest(string Label);
public record DraftStateDto(IReadOnlyList<DraftLineDto> Lines, string Priority, string? DeliveryNotes, decimal Total);
public record DraftSnapshotDto(string Label, DateTime SavedAt);

public record DispatchCommandRequest(Guid DeliveryId, string Action);
public record CommandHistoryDto(IReadOnlyList<string> History, bool CanUndo, bool CanRedo);

public record CourierIteratorRequest(string Mode, string? VehicleType, int? RoundRobinSteps);
public record CourierIteratorResponse(string Mode, IReadOnlyList<string> CourierNames);

public record DeliveryEventDto(DateTime At, Guid DeliveryId, string From, string To);
