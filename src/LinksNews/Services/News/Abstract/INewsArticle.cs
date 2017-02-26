using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinksNews.Services.News.Abstract
{
    public interface INewsArticle
    {
        NewsResponseStatus Status { get; set; }
        INewsSource Source { get; set; }
        NewsSortBy SortBy { get; set; }

        string Author { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        string Url { get; set; }
        string UrlToImage { get; set; }
        DateTimeOffset? PublishedAt { get; set; }

    }
}
/*
{
"status": "ok",
"source": "the-next-web",
"sortBy": "latest",
-"articles": [
-{
"author": "Matthew Hughes",
"title": "Stack Overflow thinks it can make resumes for developers suck less",
"description": "Stack Overflow just released a project that allows people who learned to code in a non-traditional way to demonstrate their ability.",
"url": "http://thenextweb.com/dd/2016/10/11/stack-overflow-thinks-can-make-resumes-developers-suck-less/",
"urlToImage": "https://cdn1.tnwcdn.com/wp-content/blogs.dir/1/files/2016/10/Screen-Shot-2016-10-11-at-11.54.04.png",
"publishedAt": "2016-10-11T14:30:43Z"
},
-{
"author": "Matthew Hughes",
"title": "KLM launches free in-flight newspaper app",
"description": "The KLM Media app lets you read the day's newspapers during your flight.",
"url": "http://thenextweb.com/insider/2016/10/11/klm-launches-free-in-flight-newspaper-app/",
"urlToImage": "https://cdn1.tnwcdn.com/wp-content/blogs.dir/1/files/2016/10/apptrigger1.jpg",
"publishedAt": "2016-10-11T15:21:06Z"
},
-{
"author": "Mix",
"title": "'Rich Kids' is a $1,000/month social network for attention-craving snobs",
"description": "Whether it's Facebook, Twitter or Instagram, the appeal of social media is largely its inclusivity. This isn't what this 'Rich Kids' is about.",
"url": "http://thenextweb.com/socialmedia/2016/10/11/rich-kids-social-network/",
"urlToImage": "https://cdn1.tnwcdn.com/wp-content/blogs.dir/1/files/2016/10/rich-kids-social-network.jpg",
"publishedAt": "2016-10-11T14:40:40Z"
},
-{
"author": "Juan Buis",
"title": "The UK's new five pound note has a secret musical trick",
"description": "Thanks to the United Kingdom's recently introduced five pound banknote, all Brits will soon have affordable access to a decent record player.",
"url": "http://thenextweb.com/shareables/2016/10/11/uk-five-pound-record-player/",
"urlToImage": "https://cdn1.tnwcdn.com/wp-content/blogs.dir/1/files/2016/10/note.gif",
"publishedAt": "2016-10-11T14:36:29Z"
},
-{
"author": "TNW Deals",
"title": "You can get the Lytro Illum camera at its lowest price ever — but only for the next 24 hours",
"description": "This is the future of photography: The Lytro Illum Camera lets you take “living pictures” that trump the static images even your DSLR can manage. It’s valued at nearly $1,300 and has been featured at a $349.99 sale price in our store. But for 24 hours only, you can pick up this innovative camera at …",
"url": "http://thenextweb.com/offers/2016/10/11/can-get-lytro-illum-camera-lowest-price-ever-next-24-hours/",
"urlToImage": "https://cdn1.tnwcdn.com/wp-content/blogs.dir/1/files/2016/10/blog1.jpg",
"publishedAt": "2016-10-11T14:06:19Z"
},
-{
"author": "Juan Buis",
"title": "Hinge is radically changing because it's done with swipe culture",
"description": "When you look at the online dating landscape, you see a fragmented market. Most apps are focused around swiping, but Hinge is looking to change that.",
"url": "http://thenextweb.com/apps/2016/10/11/hinge-swiping-culture/",
"urlToImage": "https://cdn1.tnwcdn.com/wp-content/blogs.dir/1/files/2016/10/hinge.png",
"publishedAt": "2016-10-11T13:12:02Z"
},
-{
"author": "Dahlia Green",
"title": "Read this if you take your startup seriously",
"description": "Why we’re only choosing 60 startups to showcase at SCALE – our startup program at TNW Momentum – when we could easily be making more money with more volume? For our fourth annual conference in New York, now called TNW Momentum, we’ve made the tough decision to limit the number of startups that make it onto the business …",
"url": "http://thenextweb.com/insider/2016/10/11/read-take-startup-seriously/",
"urlToImage": "https://cdn1.tnwcdn.com/wp-content/blogs.dir/1/files/2016/09/alta-2-1.jpg",
"publishedAt": "2016-10-11T14:24:13Z"
},
-{
"author": "Matthew Hughes",
"title": "Why I'm convinced that smartphone-only banks like Monzo are the future",
"description": "A seismic shift has taken place in the world of retail banking. Old institutions are under threat from cheaper, more convenient smartphone-only rivals.",
"url": "http://thenextweb.com/finance/2016/10/11/why-im-convinced-that-smartphone-only-banks-like-monzo-are-the-future/",
"urlToImage": "https://cdn1.tnwcdn.com/wp-content/blogs.dir/1/files/2016/10/Monzo-Mastercard-1.jpg",
"publishedAt": "2016-10-11T12:08:49Z"
},
-{
"author": "Juan Buis",
"title": "The Barnacle is the bright, yellow future of wheel clamps",
"description": "Wheel clamps -- they’ve been messing up people’s days since 1944, and haven’t seen any improvement since their conception. But now there's the Barnacle.",
"url": "http://thenextweb.com/cars/2016/10/11/barnacle-wheel-clamps/",
"urlToImage": "https://cdn1.tnwcdn.com/wp-content/blogs.dir/1/files/2016/10/barnacle.jpg",
"publishedAt": "2016-10-11T10:12:58Z"
},
-{
"author": "Mix",
"title": "Oculus wants Samsung's exploding Galaxy Note 7 nowhere near your face",
"description": "Oculus doesn't want Samsung's exploding Galaxy Note 7 anywhere near your face – and now it's banning it from its Gear VR app.",
"url": "http://thenextweb.com/mobile/2016/10/11/oculus-vr-samsung-galaxy-note-7/",
"urlToImage": "https://cdn1.tnwcdn.com/wp-content/blogs.dir/1/files/2016/10/samsung-galaxy-note-7-oculus-gear-vr-disabled.jpg",
"publishedAt": "2016-10-11T09:38:03Z"
}
]
}*/
