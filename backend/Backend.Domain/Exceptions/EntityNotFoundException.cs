namespace Backend.Domain.Exceptions;

public class EntityNotFoundException (Type entityType, List<string> fieldNames, List<string?> attemptedValues) 
    : Exception(
        $"""
         Entity with type {entityType.Name} not found by field(s): {string.Join(" | ", fieldNames)} . 
         The following values are attempted: {string.Join(" | ", attemptedValues)}
         """);