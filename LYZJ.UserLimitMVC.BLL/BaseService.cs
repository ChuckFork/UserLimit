using System;
using System.Linq;
using System.Linq.Expressions;
using LYZJ.UserLimitMVC.DAL;
using LYZJ.UserLimitMVC.IDAL;

namespace LYZJ.UserLimitMVC.BLL
{
	public abstract class BaseService<T> 
		where T : class, new()
	{
		/// <summary>
		/// 当前仓储
		/// </summary>
		public IBaseRepository<T> CurrentRepository { get; set; }

		/// <summary>
		/// DbSession的存放，为了职责单一的原则，将获取线程内唯一实例的DbSession的逻辑放到工厂里面去了
		/// </summary>
		public IDbSession DbSession = DbSessionFactory.GetCurrenntDbSession();

		/// <summary>
		/// 基类的构造函数
		/// </summary>
		protected BaseService()
		{
			SetCurrentRepository(); //构造函数里面去调用了，此设置当前仓储的抽象方法
		}

		/// <summary>
		/// 子类必须实现
		/// </summary>
		public abstract void SetCurrentRepository();

		/// <summary>
		/// 实现对数据库的添加功能
		/// </summary>
		/// <param name="entity">实体类</param>
		/// <returns>返回实体结果</returns>
		public T AddEntity(T entity)
		{
			//调用T对应的仓储来做添加工作
			var addEntity = CurrentRepository.AddEntity(entity);
			DbSession.SaveChanges();
			return addEntity;
		}

		/// <summary>
		/// 实现对数据的修改功能
		/// </summary>
		/// <param name="entity">实体类</param>
		/// <returns>如果执行成功，则返回true，否则返回false</returns>
		public bool UpdateEntity(T entity)
		{
			CurrentRepository.UpdateEntity(entity);
			return DbSession.SaveChanges() > 0;
		}

		/// <summary>
		/// 实现对数据库的修改功能
		/// </summary>
		/// <returns>返回受影响的行数</returns>
		public int UpdateEntity()
		{
			return DbSession.SaveChanges();
		}

		/// <summary>
		/// 实现对数据库的删除功能
		/// </summary>
		/// <param name="entity">实体类</param>
		/// <returns>如果执行成功，则返回true，否则返回false</returns>
		public bool DeleteEntity(T entity)
		{
			CurrentRepository.DeleteEntity(entity);
			return DbSession.SaveChanges() > 0;
		}

		/// <summary>
		/// 实现对数据库的查询  --简单查询
		/// </summary>
		/// <param name="whereLambda">查询条件</param>
		/// <returns>返回实体类的IQueryable集合</returns>
		public IQueryable<T> LoadEntities(Expression<Func<T, bool>> whereLambda)
		{
			return CurrentRepository.LoadEntities(whereLambda);
		}


		/// <summary>
		/// 实现对数据的分页查询
		/// </summary>
		/// <typeparam name="TS">按照某个类进行排序</typeparam>
		/// <param name="pageIndex">当前第几页</param>
		/// <param name="pageSize">一页显示多少条数据</param>
		/// <param name="total">总条数</param>
		/// <param name="whereLambda">取得排序的条件</param>
		/// <param name="isAsc">如何排序，根据倒叙还是升序</param>
		/// <param name="orderByLambda">根据那个字段进行排序</param>
		/// <returns>返回实体类的IQueryable集合</returns>
		public IQueryable<T> LoadPageEntities<TS>(int pageIndex, int pageSize, out int total, Expression<Func<T, bool>> whereLambda,
			bool isAsc, Expression<Func<T, TS>> orderByLambda)
		{
			return CurrentRepository.LoadPageEntities(pageIndex, pageSize, out total, whereLambda, isAsc, orderByLambda);
		}

		public int SaveChange()
		{
			return DbSession.SaveChanges();
		}

	}
}
