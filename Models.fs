module Models

type OperationResult<'a> = 
    | Success of 'a
    | Failure of string list