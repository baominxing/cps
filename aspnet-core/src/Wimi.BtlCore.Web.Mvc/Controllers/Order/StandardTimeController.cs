using System.Linq;
using System.Threading.Tasks;

using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Wimi.BtlCore.Controllers;
using Wimi.BtlCore.Order.Processes;
using Wimi.BtlCore.Order.Products;
using Wimi.BtlCore.Order.StandardTimes;
using Wimi.BtlCore.Order.StandardTimes.Dtos;
using Wimi.BtlCore.Web.Models.Order.StandardTime;

namespace Wimi.BtlCore.Web.Controllers.Order
{
    public class StandardTimeController : BtlCoreControllerBase
    {
        // private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Process> processRepository;

        private readonly IRepository<ProductGroup> productGroupRepository;

        // GET: StandardTime
        private readonly IRepository<StandardTime> standardTimeRepository;

        public StandardTimeController(
            IRepository<StandardTime> standardTimeRepository, 
            IRepository<ProductGroup> productGroupRepository, 
            IRepository<Process> processRepository)
        {
            this.standardTimeRepository = standardTimeRepository;
            this.productGroupRepository = productGroupRepository;
            this.processRepository = processRepository;
        }

        public async Task<PartialViewResult> CreateOrUpdateModal(int? id)
        {
            var model = new StandardTimeModel();
            if (id.HasValue)
            {
                var standardTime = await this.standardTimeRepository.GetAsync(id.Value);
                if (standardTime != null)
                {
                    ObjectMapper.Map(standardTime, model);
                    model.IsEditMode = true;
                }
            }

            var process = from pr in this.processRepository.GetAll()
                          select new NameValueDto<int>() { Value = pr.Id, Name = pr.Name };
            model.Process = process.ToArray();
            var productGroupQuery = this.productGroupRepository.GetAllIncluding(pg => pg.Products);
            var pgq =
                productGroupQuery.ToList()
                    .Select(
                        c =>
                        new ProductGroupAndProductsDto()
                            {
                                ProductGroupName = c.Name, 
                                Product =
                                    c.Products.Select(
                                        p => new NameValueDto<int>(p.Name, p.Id)).ToArray()
                            });
            model.ProductGroupAndProductsDto = pgq.ToArray();

            return this.PartialView("~/Views/Orders/StandardTime/_CreateOrUpdateModal.cshtml", model);
        }

        public ActionResult Index()
        {
            return this.View("~/Views/Orders/StandardTime/Index.cshtml");
        }
    }
}