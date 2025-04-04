using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ISpecification<T>
    {
        Expression<Func<T,bool>>? Criteria {get ;}
        Expression<Func<T,object>>? OrderBy {get;}
        Expression<Func<T,object>>? OrderByDescending {get;}
        List<Expression<Func<T,object>>> Includes {get;}
        List<string> IncludeStrings {get;}
        bool IsDistinct {get;}
        int Take {get;}
        int Skip {get;}
        bool IsPagingEnabeld {get;}
        IQueryable<T> ApplyCriteria(IQueryable<T> query);

    }

    public interface ISpecification<T,TResult> : ISpecification<T>{
        Expression<Func<T,TResult>>? Select {get;}
    }
}