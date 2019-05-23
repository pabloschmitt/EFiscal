using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFiscal.Common.Data
{
    public class OperationResult
    {
        private static readonly OperationResult _success = new OperationResult(true);

        public OperationResult(params string[] errors) : this((IEnumerable<string>)errors)
        {
        }

        public OperationResult(IEnumerable<string> errors)
        {
            if (errors == null)
            {
                errors = new[] { "La Operation tuvo Errores" };
            }
            Succeeded = false;
            Errors = errors;
        }

        protected OperationResult(bool success)
        {
            Succeeded = success;
            Errors = new string[0];
        }

        public bool Succeeded { get; private set; }
        public bool Omitted { get; private set; } = false;
        public IEnumerable<string> Errors { get; private set; }

        public static OperationResult Success
        {
            get { return _success; }
        }

        public static OperationResult Failed(params string[] errors)
        {
            return new OperationResult(errors);
        }

        public static OperationResult Omit(params string[] errors)
        {
            return new OperationResult() { Succeeded = false, Omitted = true, Errors = errors };
        }


    }

}
