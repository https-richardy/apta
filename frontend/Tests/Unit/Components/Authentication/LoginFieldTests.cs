namespace Apta.WebUI.Tests.Unit.Components.Authentication;

public sealed class LoginFieldTests : BunitContext
{
    public LoginFieldTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddFluentUIComponents();
    }

    [Fact(DisplayName = "[component] - when a label is provided, it is rendered above the field")]
    public void When_LabelIsProvided_Then_ItIsRendered()
    {
        /* act: render the field with a label */
        var field = Render<LoginField>(parameters => parameters
            .Add(component => component.Label, "Email"));

        /* assert: the label is displayed */
        Assert.Contains("Email", field.Markup);
    }

    [Fact(DisplayName = "[component] - when value, placeholder and type are provided, they are forwarded to the text input")]
    public void When_InputParametersAreProvided_Then_TheyAreForwardedToTheTextInput()
    {
        /* arrange: render the field with the input parameters set */
        var field = Render<LoginField>(parameters => parameters
            .Add(component => component.Value, "john.doe@apta.com.br")
            .Add(component => component.Placeholder, "seu@email.com.br")
            .Add(component => component.Type, TextInputType.Email));

        /* act: locate the inner fluent text input */
        var input = field.FindComponent<FluentTextInput>();

        /* assert: the inner input carries the provided value and placeholder */
        Assert.Equal("john.doe@apta.com.br", input.Instance.Value);
        Assert.Equal("seu@email.com.br", input.Instance.Placeholder);

        /* assert: the inner input carries the provided type */
        Assert.Equal(TextInputType.Email, input.Instance.TextInputType);
    }

    [Fact(DisplayName = "[component] - when the inner input value changes, ValueChanged is invoked with the new value")]
    public async Task When_InnerInputValueChanges_Then_ValueChangedIsInvokedWithTheNewValue()
    {
        /* arrange: render the field capturing the value change */
        string? changed = null;

        var field = Render<LoginField>(parameters => parameters
            .Add(component => component.ValueChanged, EventCallback.Factory.Create<string?>(this, value => changed = value)));

        /* act: forward a value change from the inner text input */
        var input = field.FindComponent<FluentTextInput>();

        await field.InvokeAsync(() => input.Instance.ValueChanged.InvokeAsync("typed value"));

        /* assert: the callback received the new value */
        Assert.Equal("typed value", changed);
    }
}
