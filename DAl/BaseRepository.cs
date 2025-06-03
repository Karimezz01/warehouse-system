using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using warehousesystem.data;

namespace warehousesystem.DAl
{
    public interface IRepository<T> where T : class
    {
        List<T> GetAll();
        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
    }


    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbcontext _context;

        public BaseRepository()
        {
            _context = new AppDbcontext();
        }

        public List<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = _context.Set<T>().Find(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                _context.SaveChanges();
            }
        }
    }
}