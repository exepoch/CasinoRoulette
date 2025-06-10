using System.Collections.Generic;
using UI.ViewModels;
using UI.Views;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the infinite scroll history view using object pooling.
/// Displays roulette history entries from the ViewModel.
/// </summary>
public class HistoryManager : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;           // ScrollRect component
    [SerializeField] private RectTransform content;           // Content container inside ScrollRect
    [SerializeField] private HistoryEntryPool pool;           // Object pool for HistoryEntryView
    [SerializeField] private float itemHeight = 40;           // Height of each entry
    [SerializeField] private int poolSize = 10;               // Number of views to pool

    private HistoryViewModel _viewModel;
    private readonly List<HistoryEntryView> _pooledViews = new(); // List of active pooled views
    private int _totalCount;
    private int _firstDataIndex = 0; // Index of the first visible data item

    private void Awake()
    {
        // Initialize ViewModel and register for game events
        _viewModel = new HistoryViewModel();
        _viewModel.RegisterEvents();
    }

    private void Start()
    {
        // Listen to scroll updates
        scrollRect.onValueChanged.AddListener(OnScroll);

        // Trigger scroll initialization when entries update
        _viewModel.OnEntriesUpdated += InitializeScroll;
    }

    /// <summary>
    /// Initializes the scroll system after entries are added.
    /// Sets content height and activates pooled views.
    /// </summary>
    void InitializeScroll()
    {
        _totalCount = _viewModel.Entries.Count;

        // Adjust content height based on number of entries
        content.sizeDelta = new Vector2(content.sizeDelta.x, _totalCount * itemHeight);

        pool.ResetAll();
        _pooledViews.Clear();
        _firstDataIndex = 0;

        // Instantiate enough pooled views (limited to poolSize or totalCount)
        for (int i = 0; i < Mathf.Min(poolSize, _totalCount); i++)
        {
            var view = pool.Get();
            view.transform.SetParent(content, false);
            _pooledViews.Add(view);
        }

        UpdateVisibleViews(force: true);
    }

    /// <summary>
    /// Called when the scroll position changes.
    /// Updates which entries are visible.
    /// </summary>
    void OnScroll(Vector2 _)
    {
        UpdateVisibleViews();
    }

    /// <summary>
    /// Updates the visible entry views based on scroll position.
    /// Uses recycled views and updates their data/positions.
    /// </summary>
    void UpdateVisibleViews(bool force = false)
    {
        float scrollY = content.anchoredPosition.y;
        int newFirstIndex = Mathf.Clamp(Mathf.FloorToInt(scrollY / itemHeight), 0, Mathf.Max(0, _totalCount - poolSize));

        if (!force && newFirstIndex == _firstDataIndex)
            return;

        _firstDataIndex = newFirstIndex;

        for (int i = 0; i < _pooledViews.Count; i++)
        {
            int dataIndex = _firstDataIndex + i;

            if (dataIndex >= _totalCount)
            {
                _pooledViews[i].gameObject.SetActive(false);
                continue;
            }

            var view = _pooledViews[i];
            view.gameObject.SetActive(true);
            view.SetData(_viewModel.Entries[dataIndex]);

            // Reposition entry based on its index in the full list
            var rt = view.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(50, -dataIndex * itemHeight); // 50 is X padding
        }
    }
}
