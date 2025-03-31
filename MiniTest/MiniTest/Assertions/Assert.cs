    namespace MiniTest.Assertions;

    public class Assert
    {
        public static void IsTrue(bool condition, string message = "")
        {
            if (!condition) throw new AssertException($"{message}");
        }

        public static void IsFalse(bool condition, string message = "")
        {
            if (condition) throw new AssertException($"{message}");
        }

        public static void AreEqual<T>(T? expectedObj, T? actualObj, string message = "")
        {
            if (!Equals(expectedObj, actualObj))
                throw new AssertException($"Expected: {expectedObj?.ToString() ?? "null"}. Actual: {actualObj?.ToString() ?? "null"}. {message}");
        }
        
        public static void AreNotEqual<T>(T? notExpected, T? actual, string message = "")
        {
            if (Equals(notExpected, actual))
                throw new AssertException($"Expected any value except: {notExpected?.ToString() ?? "null"}. Actual: {actual?.ToString() ?? "null"}. {message}");
        }

        public static void ThrowsException<TException>(Action action, string message = "")
            where TException : Exception
        {
            try
            {
                action();
                // program is expected to throw an exception, otherwise report a failure about no exceptions
                throw new AssertException($"Expected exception {typeof(TException).Name} but no exception was thrown. {message}");
            }
            catch (Exception exception)
            {
                // program threw an exception, check if it's the expected type
                if (exception.GetType() != typeof(TException))
                {
                    throw new AssertException($"Expected exception type:<{typeof(TException).FullName}>. Actual exception type:<{exception.GetType().FullName}>. {message}");
                }
            }
        }

        public static void Fail(string message = "")
        {
            throw new AssertException($"Test failed. {message}");
        }
    }