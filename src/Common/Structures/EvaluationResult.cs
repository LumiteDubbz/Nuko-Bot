using System;

namespace NukoBot.Common.Structures
{
    public sealed class EvaluationResult
    {
        private EvaluationResult(bool success, string result = null, Exception exception = null)
        {
            Success = success;
            Result = result;
            Exception = exception;
        }

        public static EvaluationResult FromSuccess(string result)
        {
            return new EvaluationResult(true, result);
        }

        public static EvaluationResult FromError(Exception exception)
        {
            return new EvaluationResult(true, exception: exception);
        }

        public bool Success { get; }

        public string Result { get; }
        
        public Exception Exception { get; }
    }
}
