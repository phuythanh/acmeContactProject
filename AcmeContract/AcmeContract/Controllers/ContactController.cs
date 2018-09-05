using System.Collections.Generic;
using System.IO;
using AcmeContract.FileModels;
using AcmeContract.Models;
using AcmeContract.Utilities.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AcmeContract.Controllers
{
    [Route("api/[controller]")]
    public class ContactController : Controller
    {
        private readonly ICsvReader _csvReader;
        private readonly IMapper _mapper;
        public ContactController(ICsvReader csvReader, IMapper mapper)
        {
            _csvReader = csvReader;
            _mapper = mapper;
        }
        // GET api/values
        [HttpGet]
        public List<ContactModel> Get()
        {
            var rootFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var csvFile = Path.Combine(rootFolder, @"data\ContactData.csv");
            //ContactData.csv
            var csvModel = _csvReader.ProcessCsvFile<CsvContact>(csvFile);
            var result = _mapper.Map<List<CsvContact>,List<ContactModel>>(csvModel);

            return result;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
