// Copyright (c) Bili Copilot. All rights reserved.

using System.Linq;
using Bili.Copilot.Libs.Toolkit;
using Bili.Copilot.Models.Constants.App;
using Bili.Copilot.Models.Constants.Bili;
using Bili.Copilot.Models.Constants.Community;
using Bili.Copilot.Models.Data.Appearance;
using Bili.Copilot.Models.Data.Community;
using Bili.Copilot.Models.Data.Dynamic;
using Bili.Copilot.Models.Data.User;
using Bilibili.App.Dynamic.V2;

namespace Bili.Copilot.Libs.Adapter;

/// <summary>
/// 动态数据适配器.
/// </summary>
public sealed class DynamicAdapter
{
    /// <summary>
    /// 将动态条目 <see cref="DynamicItem"/> 转换为动态信息.
    /// </summary>
    /// <param name="item">动态条目.</param>
    /// <returns><see cref="DynamicInformation"/>.</returns>
    public static DynamicInformation ConvertToDynamicInformation(DynamicItem item)
    {
        var modules = item.Modules;
        var userModule = modules.Where(p => p.ModuleType == DynModuleType.ModuleAuthor).FirstOrDefault()?.ModuleAuthor;
        var descModule = modules.Where(p => p.ModuleType == DynModuleType.ModuleDesc).FirstOrDefault()?.ModuleDesc;
        var mainModule = modules.Where(p => p.ModuleType == DynModuleType.ModuleDynamic).FirstOrDefault()?.ModuleDynamic;
        var dataModule = modules.Where(p => p.ModuleType == DynModuleType.ModuleStat).FirstOrDefault()?.ModuleStat;

        UserProfile user = default;
        string tip = default;
        EmoteText description = default;
        DynamicCommunityInformation communityInfo = default;
        var dynamicId = item.Extend.DynIdStr;
        var replyType = GetReplyTypeFromDynamicType(item.CardType);
        var replyId = replyType == CommentType.Dynamic
            ? dynamicId
            : item.Extend.BusinessId;
        var dynamicType = GetDynamicItemType(mainModule);
        var dynamicData = GetDynamicContent(mainModule);
        if (userModule != null)
        {
            var author = userModule.Author;
            user = UserAdapter.ConvertToUserProfile(author.Mid, author.Name, author.Face, AvatarSize.Size64);
            tip = userModule.PtimeLabelText;
        }
        else
        {
            var forwardUserModule = modules.Where(p => p.ModuleType == DynModuleType.ModuleAuthorForward).FirstOrDefault()?.ModuleAuthorForward;
            if (forwardUserModule != null)
            {
                var name = forwardUserModule.Title.FirstOrDefault()?.Text ?? "--";
                user = UserAdapter.ConvertToUserProfile(forwardUserModule.Uid, name, forwardUserModule.FaceUrl, AvatarSize.Size32);
                tip = forwardUserModule.PtimeLabelText;
            }
        }

        tip = TextToolkit.ConvertToTraditionalChineseIfNeeded(tip);

        if (descModule != null)
        {
            description = ImageAdapter.ConvertToEmoteText(descModule);
        }

        if (dataModule != null)
        {
            communityInfo = CommunityAdapter.ConvertToDynamicCommunityInformation(dataModule, dynamicId);
        }

        return new DynamicInformation(
            dynamicId,
            user,
            tip,
            communityInfo,
            replyType,
            replyId,
            dynamicType,
            dynamicData,
            description);
    }

    /// <summary>
    /// 将动态转发条目 <see cref="MdlDynForward"/> 转换为动态信息.
    /// </summary>
    /// <param name="forward">动态转发条目.</param>
    /// <returns><see cref="DynamicInformation"/>.</returns>
    public static DynamicInformation ConvertToDynamicInformation(MdlDynForward forward)
    {
        var item = forward.Item;
        return ConvertToDynamicInformation(item);
    }

    /// <summary>
    /// 将视频动态响应 <see cref="DynVideoReply"/> 转换为动态视图.
    /// </summary>
    /// <param name="reply">视频动态响应.</param>
    /// <returns><see cref="DynamicView"/>.</returns>
    public static DynamicView ConvertToDynamicView(DynVideoReply reply)
    {
        var list = reply.DynamicList.List.Where(p =>
            p.CardType is DynamicType.Av
            or DynamicType.Pgc
            or DynamicType.UgcSeason)
            .Select(ConvertToDynamicInformation)
            .ToList();

        var ups = reply.VideoUpList != null
            ? reply.VideoUpList.List.Select(UserAdapter.ConvertToUserProfile).ToList()
            : new System.Collections.Generic.List<DynamicUper>();
        return new DynamicView(list, ups);
    }

    /// <summary>
    /// 将综合动态响应 <see cref="DynAllReply"/> 转换为动态视图.
    /// </summary>
    /// <param name="reply">综合动态响应.</param>
    /// <returns><see cref="DynamicView"/>.</returns>
    public static DynamicView ConvertToDynamicView(DynAllReply reply)
    {
        var list = reply.DynamicList.List.Where(p =>
            p.CardType is not DynamicType.DynNone
            and not DynamicType.Ad)
            .Select(ConvertToDynamicInformation)
            .ToList();

        var ups = reply.UpList != null
            ? reply.UpList.List.Where(p => p.LiveState != LiveState.LiveLive).Select(UserAdapter.ConvertToUserProfile).ToList()
            : new System.Collections.Generic.List<DynamicUper>();
        return new DynamicView(list, ups, reply.UpList?.Footprint);
    }

    /// <summary>
    /// 将动态响应 <see cref="DynRsp"/> 转换为空间动态.
    /// </summary>
    /// <param name="reply">空间动态响应.</param>
    /// <returns><see cref="DynamicSpace"/>.</returns>
    public static DynamicSpace ConvertToDynamicSpace(DynSpaceRsp reply)
    {
        var list = reply.List.Where(p =>
            p.CardType is not DynamicType.DynNone
            and not DynamicType.Ad)
            .Select(ConvertToDynamicInformation)
            .ToList();

        return new DynamicSpace(list, reply.HistoryOffset, reply.HasMore);
    }

    private static CommentType GetReplyTypeFromDynamicType(DynamicType type)
    {
        var replyType = CommentType.None;
        switch (type)
        {
            case DynamicType.Forward:
            case DynamicType.Word:
            case DynamicType.Live:
                replyType = CommentType.Dynamic;
                break;
            case DynamicType.Draw:
                replyType = CommentType.Album;
                break;
            case DynamicType.Av:
            case DynamicType.Pgc:
            case DynamicType.UgcSeason:
            case DynamicType.Medialist:
                replyType = CommentType.Video;
                break;
            case DynamicType.Courses:
            case DynamicType.CoursesSeason:
                replyType = CommentType.Course;
                break;
            case DynamicType.Article:
                replyType = CommentType.Article;
                break;
            case DynamicType.Music:
                replyType = CommentType.Music;
                break;
            default:
                break;
        }

        return replyType;
    }

    private static DynamicItemType GetDynamicItemType(ModuleDynamic dynamic)
    {
        if (dynamic == null)
        {
            return DynamicItemType.PlainText;
        }

        var type = dynamic.Type switch
        {
            ModuleDynamicType.MdlDynArchive => dynamic.DynArchive.IsPGC
                ? DynamicItemType.Pgc
                : DynamicItemType.Video,
            ModuleDynamicType.MdlDynPgc => DynamicItemType.Pgc,
            ModuleDynamicType.MdlDynForward => DynamicItemType.Forward,
            ModuleDynamicType.MdlDynDraw => DynamicItemType.Image,
            ModuleDynamicType.MdlDynArticle => DynamicItemType.Article,
            _ => DynamicItemType.Unsupported,
        };

        return type;
    }

    private static object GetDynamicContent(ModuleDynamic dynamic)
    {
        if (dynamic == null)
        {
            return null;
        }

        if (dynamic.Type == ModuleDynamicType.MdlDynPgc)
        {
            return PgcAdapter.ConvertToEpisodeInformation(dynamic.DynPgc);
        }
        else if (dynamic.Type == ModuleDynamicType.MdlDynArchive)
        {
            return dynamic.DynArchive.IsPGC
                ? PgcAdapter.ConvertToEpisodeInformation(dynamic.DynArchive)
                : VideoAdapter.ConvertToVideoInformation(dynamic.DynArchive);
        }
        else if (dynamic.Type == ModuleDynamicType.MdlDynForward)
        {
            return ConvertToDynamicInformation(dynamic.DynForward);
        }
        else if (dynamic.Type == ModuleDynamicType.MdlDynDraw)
        {
            return dynamic.DynDraw.Items.Select(p => ImageAdapter.ConvertToImage(p.Src, 240d, 240d)).ToList();
        }
        else if (dynamic.Type == ModuleDynamicType.MdlDynArticle)
        {
            return ArticleAdapter.ConvertToArticleInformation(dynamic.DynArticle);
        }

        return null;
    }
}
