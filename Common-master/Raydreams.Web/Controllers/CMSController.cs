using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Raydreams.Common.Services.Model;
using Raydreams.Web.Data;

namespace Raydreams.Web.Controllers
{
    /// <summary>The CMS specific controller</summary>
    public class CMSController : Controller
    {
        /// <summary>The main API repo</summary>
        private CMSRepository _repo = null;

        #region [ Constructor ]

        public CMSController()
        {
            // setup the repos
            this.Repo = new CMSRepository();
        }

        #endregion [ Constructor ]

        #region [ Properties ]

        /// <summary>Repo to the BP Common Core</summary>
        protected CMSRepository Repo
        {
            get { return this._repo; }
            set { this._repo = value; }
        }

        #endregion [ Properties ]

        /// <summary></summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [Route( "cms/index" )]
        public IActionResult Index( string page )
        {
            ViewData["Title"] = "All Pages";
            List<ContentHeader> pages = this.Repo.GetContentList();
            return View( pages );
        }

        /// <summary></summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [Route( "cms/page/{path}/{lang?}" )]
        public IActionResult Page( string path, string lang )
        {
            ContentPage content = this.Repo.GetContent( path, lang );
            ViewData["Title"] = content.Locales[0].Title;
            return View( content );
        }

        /// <summary></summary>
        /// <param name="page"></param>
        /// <returns></returns>
        //[Route( "cms/{path}/{lang?}" )]
        //public IActionResult Definition( string page )
        //{
        //    ContentPage content = this.Repo.GetContent( page );
        //    ViewData["Title"] = content.Locales[0].Title;
        //    return View( content );
        //}
    }
}
