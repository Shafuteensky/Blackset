using System.Collections.Generic;
using Blackset.DuelContracts.HoverInfo;
using Extensions.Data.InMemoryData;
using Extensions.Log;
using UnityEngine;

namespace Blackset.ObjectDistribution
{
    /// <summary>
    /// Абстракция распределителя инстанцированных элементов по точкам
    /// </summary>
    /// <typeparam name="TPrefab">Тип префаба для спавна</typeparam>
    /// <typeparam name="TFactory">Тип фабрики</typeparam>
    public abstract class BaseFactoryObjectsDistributor<TPrefab, TFactory> : MonoBehaviour
        where TPrefab : MonoBehaviour
        where TFactory : BaseInMemoryDataFactory<TPrefab>  
    {
        [Header("Фабрика"), Space]
        [SerializeField] protected TFactory factory;
        [SerializeField] protected Transform factoryRoot;
        [SerializeField] protected bool clearOnEnable = true;

        [Header("Точки распределения"), Space]
        [SerializeField] protected List<Transform> points = new();

        protected int nextPointIndex;
        protected bool isWrapWarningLogged;
        protected int[] shuffledPointIndices;

        protected virtual void OnEnable()
        {
            if (factory == null)
            {
                ServiceDebug.LogError("Ссылка на фабрику не назначена, объекты не распределены");
                return;
            }

            if (factoryRoot == null)
            {
                ServiceDebug.LogError("FactoryRoot не назначен, объекты не распределены");
                return;
            }

            if (points == null || points.Count == 0)
            {
                ServiceDebug.LogError("Не объявлены точки распределения, объекты не распределены");
                return;
            }

            // Очистка существующих
            if (clearOnEnable)
            {
                foreach (Transform child in factoryRoot)
                    Destroy(child.gameObject);
            }

            // Детерминированно готовим shuffle по текущему списку EntryId
            BuildDeterministicShuffleFromRoot();
            nextPointIndex = 0;
            isWrapWarningLogged = false;
            
            factory.onObjectInstantiated += Place;
            
            // Раскладываем уже существующие элементы под root
            if (!clearOnEnable) DistributeToPoint();
        }
        
        protected virtual void OnDisable()
        {
            if (factory == null) return;
            factory.onObjectInstantiated -= Place;
        }

        /// <summary>
        /// Распределить объекты в корне по заданным случайным координатам
        /// </summary>
        public void DistributeToPoint()
        {
            foreach (Transform child in factoryRoot)
            {
                if (child == null) continue;

                TPrefab element = child.GetComponent<TPrefab>();
                if (element == null)
                {
                    continue;
                }

                Place(element);
            }
        }

        protected void Place(TPrefab element)
        {
            if (element == null)
            {
                ServiceDebug.LogError("Элемент невалиден, объект не распределен");
                return;
            }

            if (shuffledPointIndices == null || shuffledPointIndices.Length == 0)
            {
                ServiceDebug.LogError("Точки не подготовлены (shuffle пуст), объект не распределен");
                return;
            }

            if (nextPointIndex >= points.Count && !isWrapWarningLogged)
            {
                ServiceDebug.LogWarning($"Точек объявлено меньше ({points.Count}), чем объектов: объекты распределяются по кругу");
                isWrapWarningLogged = true;
            }

            int orderIndex = nextPointIndex % points.Count;
            int pointIndex = shuffledPointIndices[orderIndex];

            Transform targetPoint = points[pointIndex];
            if (targetPoint == null)
            {
                ServiceDebug.LogWarning($"Задана невалидная точка (индекс {pointIndex})");
                nextPointIndex++;
                return;
            }

            Transform t = element.gameObject.transform;
            t.SetPositionAndRotation(targetPoint.position, targetPoint.rotation);

            nextPointIndex++;
        }

        #region Детерминированное случайное распределение
        
        protected void BuildDeterministicShuffleFromRoot()
        {
            int seed = ComputeSeedFromOpponentsIds(factoryRoot);

            shuffledPointIndices = new int[points.Count];
            for (int i = 0; i < shuffledPointIndices.Length; i++)
            {
                shuffledPointIndices[i] = i;
            }

            System.Random rng = new System.Random(seed);

            for (int i = shuffledPointIndices.Length - 1; i > 0; i--)
            {
                int j = rng.Next(0, i + 1);
                (shuffledPointIndices[i], shuffledPointIndices[j]) = (shuffledPointIndices[j], shuffledPointIndices[i]);
            }
        }

        protected int ComputeSeedFromOpponentsIds(Transform root)
        {
            unchecked
            {
                uint hash = 2166136261u; // FNV-1a
                const uint prime = 16777619u;

                int count = root.childCount;
                hash ^= (uint)count; hash *= prime;

                for (int i = 0; i < count; i++)
                {
                    Transform child = root.GetChild(i);
                    if (child == null) continue;

                    ContractEntryVisualElement element = child.GetComponent<ContractEntryVisualElement>();
                    if (element == null) continue;

                    string id = element.DataContainer.GetOpponentData(element.EntryId).Id ?? string.Empty;

                    foreach (var t in id)
                    {
                        hash ^= t;
                        hash *= prime;
                    }

                    hash ^= '|';
                    hash *= prime;
                }

                return (int)hash;
            }
        }
        
        #endregion
    }
}