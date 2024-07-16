using Richasy.BiliKernel;
using Richasy.BiliKernel.Bili.Article;
using Richasy.BiliKernel.Models.Article;
using Spectre.Console;

namespace Bili.Console;

internal sealed class ArticleModule : IFeatureModule
{
    private readonly Kernel _kernel;
    private readonly CancellationToken _cancellationToken;
    private readonly Func<string, Task> _backFunc;
    private readonly IArticleDiscoveryService _articleService;

    // private IList<Partition> _partitions;

    public ArticleModule(
        Kernel kernel,
        CancellationToken cancellationToken,
        Func<string, Task> backFunc)
    {
        _kernel = kernel;
        _cancellationToken = cancellationToken;
        _backFunc = backFunc;
        _articleService = kernel.GetRequiredService<IArticleDiscoveryService>();
    }

    public async Task RunAsync()
    {
#if PARTITION
        if (_partitions == null)
        {
            var partitions = await _articleService.GetPartitionsAsync(_cancellationToken).ConfigureAwait(false);
            _partitions = partitions.ToList();
        }

        var partition = AnsiConsole.Prompt(
            new SelectionPrompt<Partition>()
                .Title("请选择文章分区")
                .PageSize(10)
                .AddChoices(_partitions)
                .UseConverter(p => p.Name));

        AnsiConsole.Clear();

        AnsiConsole.MarkupLine("[bold]推荐文章[/]");
        var (recArticles, topArticles, _) = await _articleService.GetRecommendArticlesAsync().ConfigureAwait(false);
        DisplayArticles(recArticles);

        if (topArticles != null)
        {
            AnsiConsole.MarkupLine("[bold]热门文章[/]");
            DisplayArticles(topArticles);
        }

        AnsiConsole.MarkupLine($"{partition.Name} 文章");
        var (partArticles, _) = await _articleService.GetPartitionArticlesAsync(partition, ArticleSortType.Default).ConfigureAwait(false);
        DisplayArticles(partArticles);

        var firstArticle = topArticles?.FirstOrDefault() ?? recArticles.FirstOrDefault() ?? partArticles.FirstOrDefault();
        if (firstArticle != null)
        {
            AnsiConsole.MarkupLine($"[bold]{firstArticle.Identifier.Title}[/]");
            var content = await _articleService.GetArticleContentAsync(firstArticle.Identifier, _cancellationToken).ConfigureAwait(false);
            AnsiConsole.MarkupLine(Markup.Escape(content));
        }
#endif
        var hotCategories = await _articleService.GetHotCategoriesAsync(_cancellationToken).ConfigureAwait(false);
        var categoryId = AnsiConsole.Prompt(
            new SelectionPrompt<KeyValuePair<int, string>>()
                .Title("请选择热门专栏分区")
                .PageSize(10)
                .AddChoices(hotCategories.ToList())
                .UseConverter(p => p.Value)).Key;

        AnsiConsole.Clear();
        AnsiConsole.MarkupLine($"正在获取 {hotCategories[categoryId]} 热门专栏文章...");
        var hotArticles = await _articleService.GetHotArticlesAsync(categoryId, _cancellationToken).ConfigureAwait(false);
        AnsiConsole.Clear();
        DisplayArticles(hotArticles);

        if (AnsiConsole.Confirm("是否返回？"))
        {
            await _backFunc(string.Empty).ConfigureAwait(false);
        }
    }

    private static void DisplayArticles(IReadOnlyList<ArticleInformation> articles)
    {
        var table = new Table();
        table.AddColumn("标题");
        table.AddColumn("作者");
        table.AddColumn("发布时间");

        foreach (var item in articles)
        {
            table.AddRow(Markup.Escape(item.Identifier.Title), Markup.Escape(item.Publisher.Name), item.PublishDateTime.ToString());
        }

        AnsiConsole.Write(table);
    }
}
