using System.Collections.Generic;
using UI.Views;
using UnityEngine;

/// <summary>
/// Object pool for reusing HistoryEntryView instances efficiently.
/// Avoids unnecessary instantiations during scrolling.
/// </summary>
public class HistoryEntryPool : MonoBehaviour
{
    [SerializeField] private HistoryEntryView prefab; // Prefab to pool
    [SerializeField] private int poolSize = 10;       // Number of objects to create in the pool

    private readonly Queue<HistoryEntryView> pool = new();

    private void Awake()
    {
        // Pre-instantiate pooled objects and deactivate them
        for (int i = 0; i < poolSize; i++)
        {
            var view = Instantiate(prefab, transform);
            view.gameObject.SetActive(false);
            pool.Enqueue(view);
        }
    }

    /// <summary>
    /// Retrieves an available view from the pool.
    /// Immediately re-enqueues it to cycle through objects.
    /// </summary>
    public HistoryEntryView Get()
    {
        var view = pool.Dequeue();
        view.gameObject.SetActive(true);
        pool.Enqueue(view);
        return view;
    }

    /// <summary>
    /// Deactivates all pooled views (typically on re-initialization).
    /// </summary>
    public void ResetAll()
    {
        foreach (var view in pool)
            view.gameObject.SetActive(false);
    }
}