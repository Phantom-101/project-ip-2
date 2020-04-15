using NUnit.Framework;

namespace Unity.DOTS.Editor.Tests
{
    [TestFixture]
    class EntityConversionPreviewTests
    {
        // @todo @antoineb uncomment these tests when DOTS.Editor.Tests have access to entities internals
        // [Test]
        // public void EntityConversionPreview_ShowOnlyLiveAndConversionWorlds()
        // {
        //     using (WorldScope.Create())
        //     {
        //         using (var live = new World("live", WorldFlags.Live))
        //         using (var conversion = new World("conversion", WorldFlags.Conversion))
        //         using (var shadow = new World("shadow", WorldFlags.Shadow))
        //         using (var staging = new World("staging", WorldFlags.Staging))
        //         using (var streaming = new World("streaming", WorldFlags.Streaming))
        //         {
        //             Assert.That(EntityConversionPreview.Worlds.FilteredWorlds, Does.Contain(live));
        //             Assert.That(EntityConversionPreview.Worlds.FilteredWorlds, Does.Contain(conversion));
        //
        //             Assert.That(EntityConversionPreview.Worlds.FilteredWorlds, Does.Not.Contains(shadow));
        //             Assert.That(EntityConversionPreview.Worlds.FilteredWorlds, Does.Not.Contains(staging));
        //             Assert.That(EntityConversionPreview.Worlds.FilteredWorlds, Does.Not.Contains(streaming));
        //         }
        //     }
        // }
        //
        // [Test]
        // public void EntityConversionPreview_ShowCreatedWorlds()
        // {
        //     using (WorldScope.Create())
        //     {
        //         Assert.That(EntityConversionPreview.Worlds.FilteredWorlds, Is.Empty);
        //
        //         using (new World("test", WorldFlags.Live))
        //         {
        //             Assert.That(EntityConversionPreview.Worlds.FilteredWorlds, Is.Not.Empty);
        //         }
        //     }
        // }
        //
        // struct WorldScope : IDisposable
        // {
        //     World[] m_CapturedWorlds;
        //
        //     public static WorldScope Create()
        //     {
        //         var scope = new WorldScope { m_CapturedWorlds = World.s_AllWorlds.ToArray() };
        //         World.s_AllWorlds.Clear();
        //         return scope;
        //     }
        //
        //     public void Dispose()
        //     {
        //         World.s_AllWorlds.Clear();
        //         World.s_AllWorlds.AddRange(m_CapturedWorlds);
        //     }
        // }
    }
}