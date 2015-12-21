﻿using JayData.Test.CommonItems.Entities;
using JayData.Test.WebApi_2_2_OData_4.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace WebApi_2_2_OData_4.Controllers
{
    public class UserProfilesController : ODataController
    {
        NewsReaderContext db = new NewsReaderContext();

        [EnableQuery]
        public IQueryable<UserProfile> Get()
        {
            return db.UserProfiles;
        }

        [EnableQuery]
        public SingleResult<UserProfile> Get([FromODataUri] int key)
        {
            IQueryable<UserProfile> result = db.UserProfiles.Where(p => p.Id == key);
            return SingleResult.Create(result);
        }

        public async Task<IHttpActionResult> Post(UserProfile UserProfile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.UserProfiles.Add(UserProfile);
            await db.SaveChangesAsync();
            return Created(UserProfile);
        }

        public async Task<IHttpActionResult> Patch([FromODataUri] int key, Delta<UserProfile> UserProfile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var entity = await db.UserProfiles.FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }
            UserProfile.Patch(entity);
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(entity);
        }
        
        public async Task<IHttpActionResult> Put([FromODataUri] int key, UserProfile update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (key != update.Id)
            {
                return BadRequest();
            }
            db.Entry(update).State = EntityState.Modified;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(update);
        }
        
        public async Task<IHttpActionResult> Delete([FromODataUri] int key)
        {
            var UserProfile = await db.UserProfiles.FindAsync(key);
            if (UserProfile == null)
            {
                return NotFound();
            }
            db.UserProfiles.Remove(UserProfile);
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }

        private bool ProductExists(int key)
        {
            return db.UserProfiles.Any(p => p.Id == key);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
