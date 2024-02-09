using System;

namespace Weather.Behaviors;

public class FlexLayoutBehavior : Behavior<FlexLayout>
{
    private FlexLayout view;

    private void SetState(VisualElement view, string state)
    {
        VisualStateManager.GoToState(view, state);
        if (view is Layout layout)
        {
            foreach (VisualElement child in layout.Children)
            {
                SetState(child, state);
            }
        }
    }

    private void UpdateState()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var page = Application.Current.MainPage;

            if (page.Width > page.Height)
            {
                SetState(view, "Landscape");
                return;
            }

            SetState(view, "Portrait");
        });
    }

    protected override void OnAttachedTo(FlexLayout view)
    {
        this.view = view;
        base.OnAttachedTo(view);
        UpdateState();
        Application.Current.MainPage.SizeChanged += MainPage_SizeChanged;
    }

    void MainPage_SizeChanged(object sender, EventArgs e)
    {
        UpdateState();
    }

    protected override void OnDetachingFrom(FlexLayout view)
    {
        base.OnDetachingFrom(view);
        Application.Current.MainPage.SizeChanged -= MainPage_SizeChanged;
        this.view = null;
    }


}
