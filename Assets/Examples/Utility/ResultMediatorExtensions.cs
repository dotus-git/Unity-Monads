// using Monads;
// using UniMediator;
//
// public static class ResultMediatorExtensions
// {
//     public static Result<TOut> Send<TIn, TOut>(
//         this Result<TIn> result,
//         ISingleMessage<Result<TOut>> message)
//     {
//         return result.IsSuccess
//             ? DataMediator.Instance.Send(message)
//             : new Result<TOut>(result);
//     }
//     
//     public static Result Send(ISingleMessage<Result> message)
//     {
//         return DataMediator.Instance.Send(message);
//     }
// }