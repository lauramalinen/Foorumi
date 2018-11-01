﻿using Foorumi.Models;
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

namespace Foorumi.Controllers
{
    public class AlueetController : ApiController
    {
        private FoorumiModel db = new FoorumiModel();

        // GET: api/Alueet
        public IQueryable<Alue> GetAlueet()
        {
            Kayttaja k = Kirjautuminen.HaeKirjautuminen(Request.Headers.Authorization?.ToString().Substring(7) ?? null);
            return db.Alueet.Where(a => a.NakeekoKayttaja(k));
        }

        // GET: api/Alueet/5
        [ResponseType(typeof(Lanka))]
        public IHttpActionResult GetAlue(int id)
        {
            Kayttaja k = Kirjautuminen.HaeKirjautuminen(Request.Headers.Authorization?.ToString().Substring(7) ?? null);
            Alue alue = db.Alueet.Find(id);
            if (alue == null)
            {
                return NotFound();
            }

            if (!alue.NakeekoKayttaja(k))
            {
                return Unauthorized();
            }

            return Ok(new
            {
                alue,
                langat = alue.Langat
            });
        }

        // PUT: api/Alueet/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAlue(int id, Alue alue)
        {
            Kayttaja k = Kirjautuminen.HaeKirjautuminen(Request.Headers.Authorization?.ToString().Substring(7) ?? null);
            if (!k.Kayttajatasot.o_alueMuokkaa)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != alue.alue_id)
            {
                return BadRequest();
            }

            db.Entry(alue).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlueExists(id))
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

        // POST: api/Alueet
        [ResponseType(typeof(Alue))]
        public IHttpActionResult PostAlue(Alue alue)
        {
            Kayttaja k = Kirjautuminen.HaeKirjautuminen(Request.Headers.Authorization?.ToString().Substring(7) ?? null);
            if (!k.Kayttajatasot.o_alueLisaa)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Alueet.Add(alue);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = alue.alue_id }, alue);
        }

        // DELETE: api/Alueet/5
        [ResponseType(typeof(Alue))]
        public IHttpActionResult DeleteAlue(int id)
        {
            Kayttaja k = Kirjautuminen.HaeKirjautuminen(Request.Headers.Authorization?.ToString().Substring(7) ?? null);
            if (!k.Kayttajatasot.o_aluePoista)
            {
                return Unauthorized();
            }

            Alue alue = db.Alueet.Find(id);
            if (alue == null)
            {
                return NotFound();
            }

            db.Alueet.Remove(alue);
            db.SaveChanges();

            return Ok(alue);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AlueExists(int id)
        {
            return db.Alueet.Count(e => e.alue_id == id) > 0;
        }
    }
}