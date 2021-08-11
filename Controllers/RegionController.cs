﻿using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YaaranutGisApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RegionController : BaseController
    {
        public RegionController(YaaranutGisApi.IAppSettings appSettings, IGisApiHelper GisApiHelper) : base(appSettings, GisApiHelper) { }

        [HttpGet]
        [Route("GetRegions")]
        public async Task<ActionResult<IEnumerable<GisRegionModel.RegionModel>>> GetRegions()
        {
            IList<GisRegionModel.RegionModel> regions = new List<GisRegionModel.RegionModel>();

            var reqparmForest = new System.Collections.Specialized.NameValueCollection
                {
                    {"where", "1=1" },
                    {"outFields", YaaranutGisApi.GisApiHelper.GetModelFields(typeof( GisRegionModel.RegionModel))},
                    {"returnGeometry", "false"},
                    {"returnExceededLimitFeatures", "true"},
                    {"token", this.GisApiHelper.GetToken()},
                    {"f", "json"},
                    {"geometryType","esriGeometryPoint"},
                };

            var Gisfeatures = System.Text.Json.JsonSerializer.Deserialize<GisRegionModel>(this.GisApiHelper.GetFeatures("JNFRegions",0, reqparmForest));
            if (Gisfeatures.error == null)
            {
                foreach (var item in Gisfeatures.features)
                {
                    regions.Add(item.attributes);
                }
                return Ok(regions);
            }
            else
            {
                return StatusCode(500, Gisfeatures.error.message + " " + Gisfeatures.error.details[0] + " where:" + reqparmForest.GetValues("where")[0] + " Fields:" + reqparmForest.GetValues("outFields")[0]);
            }
        }
    }

    public class GisRegionModel : GisModel
    {
        public Features[] features { get; set; }
        public class Features
        {
            public Attributes attributes { get; set; }
        }
        public class Attributes : RegionModel
        {
        }
        public class RegionModel
        {
            public int? OBJECTID { get; set; }
            public int merchav_co { get; set; }
            public int ezor_code { get; set; }
            public string ezor_name { get; set; }
        }
    }
}