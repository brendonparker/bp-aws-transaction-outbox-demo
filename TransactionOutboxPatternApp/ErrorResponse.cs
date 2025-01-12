namespace TransactionOutboxPatternApp;

public class ErrorResponse
{
    public static readonly ErrorResponse AlreadySubmitted = new()
    {
        ErrorMessage = "Order has already been submitted"
    };

    public required string ErrorMessage { get; init; }
}