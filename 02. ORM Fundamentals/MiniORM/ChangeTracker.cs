using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace MiniORM
{
    internal class ChangeTracker<T>
        where T: class, new()
    {
        private readonly List<T> allEntities;

        private readonly List<T> addedEntities;

        private readonly List<T> removedEntities;



        public ChangeTracker(IEnumerable<T> entities)
        {
            this.addedEntities = new List<T>();
            this.removedEntities = new List<T>();

            this.allEntities = CloneEntities(entities);
        }



        public IReadOnlyCollection<T> AllEntities => this.allEntities.AsReadOnly();

        public IReadOnlyCollection<T> AddedEntities => this.addedEntities.AsReadOnly();

        public IReadOnlyCollection<T> RemovedEntities => this.removedEntities.AsReadOnly();



        private static List<T> CloneEntities(IEnumerable<T> entities)
        {
            var clonedEntities = new List<T>();

            var propertiesToClone = typeof(T).GetProperties()
                .Where(p => DbContext.AllowedSqlTypes.Contains(p.PropertyType))
                .ToArray();

            foreach (var entity in entities)
            {
                var cloneEntity = Activator.CreateInstance<T>();

                foreach (var property in propertiesToClone)
                {
                    var value = property.GetValue(entity);
                    property.SetValue(cloneEntity, value);
                }

                clonedEntities.Add(cloneEntity);
            }

            return clonedEntities;
        }

        public void Add(T item)
        {
            this.addedEntities.Add(item);
        }

        public void Remove(T item)
        {
            this.removedEntities.Add(item);
        }

        public IEnumerable<T> GetModifiedEntities(DbSet<T> dbSet)
        {
            var modifiedEntities = new List<T>();

            var primaryKeys = typeof(T).GetProperties()
                .Where(p => p.HasAttribute<KeyAttribute>())
                .ToArray();

            foreach (var proxyEntity in this.allEntities)
            {
                var primaryKeyValues = GetPrimaryKeyValues(primaryKeys, proxyEntity).ToArray();

                var entity = dbSet.Entities
                    .Single(e => GetPrimaryKeyValues(primaryKeys, e).SequenceEqual(primaryKeyValues));

                var isModified = IsModified(proxyEntity, entity);

                if (isModified)
                {
                    modifiedEntities.Add(entity);
                }
            }

            return modifiedEntities;
        }

        private static bool IsModified(T entity, T proxyEntity)
        {
            var monitoredProperties = typeof(T)
                .GetProperties()
                .Where(p => DbContext.AllowedSqlTypes.Contains(p.PropertyType));

            var modifiedProperties = monitoredProperties
                .Where(p => !Equals(p.GetValue(entity), p.GetValue(proxyEntity)))
                .ToArray();

            var isModified = modifiedProperties.Any();

            return isModified;
        }

        private static IEnumerable<object> GetPrimaryKeyValues(IEnumerable<PropertyInfo> primaryKeys, T entity)
        {
            return primaryKeys.Select(pk => pk.GetValue(entity));
        }
    }
}