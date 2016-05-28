using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using AuctionManager.Models;

namespace AuctionManager.Controllers
{
    public class AuctionsController : ApiController
    {
        private AuctionsDB db = new AuctionsDB();

        // GET: api/Auctions
        public IQueryable<Auction> Getauctions()
        {
            return db.auctions;
        }

        // GET: api/Auctions/5
        [ResponseType(typeof(Auction))]
        public IHttpActionResult GetAuction(int id)
        {
            Auction auction = db.auctions.Find(id);
            if (auction == null)
            {
                return NotFound();
            }

            return Ok(auction);
        }

        // PUT: api/Auctions/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAuction(int id, Auction auction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != auction.Id)
            {
                return BadRequest();
            }

            db.Entry(auction).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuctionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Auctions
        [ResponseType(typeof(Auction))]
        public IHttpActionResult PostAuction(Auction auction)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.auctions.Add(auction);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = auction.Id }, auction);
        }

        // DELETE: api/Auctions/5
        [ResponseType(typeof(Auction))]
        public IHttpActionResult DeleteAuction(int id)
        {
            Auction auction = db.auctions.Find(id);
            if (auction == null)
            {
                return NotFound();
            }

            db.auctions.Remove(auction);
            db.SaveChanges();

            return Ok(auction);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AuctionExists(int id)
        {
            return db.auctions.Count(e => e.Id == id) > 0;
        }
    }
}