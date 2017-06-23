using CsQuery;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace CNN.Controllers
{
    public class Article
    {
        public string title;
        public string url;
        public string img_url;
        public Article()
        {
            title = "";
            url = "";
            img_url = "";
        }
    }
    public class CNNController : Controller
    {
        // GET: CNN
        public ActionResult Index()
        {
            ArrayList articles = new ArrayList();
            WebClient webClient = new WebClient();
            webClient.Headers.Add("user-agent", "Only a test!");
            const string strUrl = "https://edition.cnn.com/middle-east";
            string pageContent = webClient.DownloadString(strUrl);
            CQ dom = pageContent;
            CQ top_news = dom.Find("ul.cn-list-hierarchical-piped").Eq(0);
            
            Article t_news = new Article();
            t_news.img_url = top_news.Find("img").Attr("data-src-large").ToString();
            top_news = top_news.Find("h3.cd__headline").Eq(0);
            t_news.title = top_news.Find("span").Text().ToString();
            t_news.url = top_news.Find("a").Attr("href");
            if (!t_news.url.Contains("http:")) t_news.url = "https://edition.cnn.com" + t_news.url;
            if ( !t_news.img_url.Contains("http:"))
            {
                t_news.img_url = "http://" + t_news.img_url;

            }
            articles.Add(t_news);

            CQ t_stories = dom.Find("ul.cn-list-xs").Eq(1);
            for ( int i = 0; i < t_stories.Children("li").Length; i++)
            {
                CQ story = t_stories.Children("li").Eq(i);
                Article news = new Article();
                news.url= story.Find("a").Eq(0).Attr("href").ToString();
                if (!news.url.Contains("http:")) news.url= "https://edition.cnn.com" + news.url;
                news.title = story.Find("span.cd__headline-text").Text().ToString();
                articles.Add(news);
            }

            ViewBag.newsList = articles;

            return View();
        }
        public ActionResult detail(string url)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add("user-agent", "Only a test!");
            string strUrl = url;
            string pageContent = webClient.DownloadString(strUrl);
            CQ dom = pageContent;
            CQ article_body = dom.Find("div.pg-right-rail-tall").Eq(0).Find("article").Eq(0).Find("div.l-container").Eq(0);
            ViewBag.title = article_body.Children("h1").Text().ToString();
            article_body = article_body.Children("div.pg-rail-tall__wrapper").Eq(0).Children("div.pg-side-of-rail").Eq(0);
            CQ temp = article_body.Find("div.media__video--thumbnail-wrapper").Eq(0);
            temp.Remove();
            temp = article_body.Find(".el__leafmedia.el__leafmedia--storyhighlights").Eq(0);
            temp.Remove();
            temp = article_body.Find(".el__embedded.el__embedded--standard");
            temp.Remove();
            temp = article_body.Find(".el__gallery--expandable.js__gallery--expandable.js__leafmedia--gallery");
            temp.Remove();
            temp = article_body.Find(".cn.cn-list-hierarchical-small-horizontal.cn--idx-0.cn-");
            temp.Remove();
            temp = article_body.Find("input.sharebar-video-embed-field");
            temp.Remove();
            temp = article_body.Find(".zn__containers");
            temp.Remove();
            temp = article_body.Find("img.media__image.media__image--responsive");
            for (  int i = 0; i < temp.Length; i++)
            {
                CQ t = temp.Eq(i);
                string str = t.Attr("data-src-large");
                t.AttrSet( new { src = str });
            }
            temp = article_body.Find(".cn__column.carousel__content__item");
            temp.Remove();
            ViewBag.text = article_body.RenderSelection().ToString();
            
            return View();
        }
    }
}