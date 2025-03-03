using Microsoft.AspNetCore.Mvc;
using SHERIA.Helpers;
using SHERIA.Models;
using System.Data;

namespace SHERIA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private static int saltLengthLimit = 32;
        private IWebHostEnvironment ihostingenvironment;
        private ILoggerManager iloggermanager;
        private DBHandler dbhandler;

        public EventsController(ILoggerManager logger, IWebHostEnvironment environment, DBHandler mydbhandler)
        {
            iloggermanager = logger;
            ihostingenvironment = environment;
            dbhandler = mydbhandler;
        }

        // GET: api/CalendarEvents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CalendarEventModel>>> GetEvents([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            List<CalendarEventModel> recordlist = new List<CalendarEventModel>();
            try
            {
                DataTable dt = new DataTable();

                dt = dbhandler.GetRecords("calender_tasks_record");
                foreach (DataRow dr in dt.Rows)
                {
                    recordlist.Add(
                        new CalendarEventModel
                        {
                            Id = Convert.ToInt64(dr["id"]),
                            Start = Convert.ToDateTime(dr["start_date"]),
                            End = Convert.ToDateTime(dr["due_date"]),
                            Text = Convert.ToString(dr["status"]),
                            Color = Convert.ToString(dr["color_status"])!
                        });
                }

                return recordlist;
            }
            catch (Exception ex)
            {
                FileLogHelper.log_message_fields("ERROR", "GetTasks | Exception ->" + ex.Message);
            }
            return BadRequest();
        }

        // GET: api/Events/5
        //    [HttpGet("{id}")]
        //    public async Task<ActionResult<CalendarEventModel>> GetCalendarEvent(int id)
        //    {
        //        var calendarEvent = await _context.Events.FindAsync(id);

        //        if (calendarEvent == null)
        //        {
        //            return NotFound();
        //        }

        //        return calendarEvent;
        //    }

        //    // PUT: api/Events/5
        //    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //    [HttpPut("{id}")]
        //    public async Task<IActionResult> PutCalendarEvent(int id, CalendarEventModel calendarEvent)
        //    {
        //        if (id != calendarEvent.Id)
        //        {
        //            return BadRequest();
        //        }

        //        _context.Entry(calendarEvent).State = EntityState.Modified;

        //        try
        //        {
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CalendarEventExists(id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }

        //        return NoContent();
        //    }

        //    // POST: api/Events
        //    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //    [HttpPost]
        //    public async Task<ActionResult<CalendarEventModel>> PostCalendarEvent(CalendarEventModel calendarEvent)
        //    {
        //        _context.Events.Add(calendarEvent);
        //        await _context.SaveChangesAsync();

        //        return CreatedAtAction("GetCalendarEvent", new { id = calendarEvent.Id }, calendarEvent);
        //    }

        //    // DELETE: api/Events/5
        //    [HttpDelete("{id}")]
        //    public async Task<IActionResult> DeleteCalendarEvent(int id)
        //    {
        //        var calendarEvent = await _context.Events.FindAsync(id);
        //        if (calendarEvent == null)
        //        {
        //            return NotFound();
        //        }

        //        _context.Events.Remove(calendarEvent);
        //        await _context.SaveChangesAsync();

        //        return NoContent();
        //    }

        //    private bool CalendarEventExists(int id)
        //    {
        //        return _context.Events.Any(e => e.Id == id);
        //    }

        //    // PUT: api/Events/5/move
        //    [HttpPut("{id}/move")]
        //    public async Task<IActionResult> MoveEvent([FromRoute] int id, [FromBody] EventMoveParams param)
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        var @event = await _context.Events.SingleOrDefaultAsync(m=> m.Id == id);
        //        if (@event == null)
        //        {
        //            return NotFound();
        //        }

        //        @event.Start = param.Start;
        //        @event.End = param.End;

        //        try
        //        {
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CalendarEventExists(id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }

        //        return NoContent();
        //    }

        //    // PUT: api/Events/5/color
        //    [HttpPut("{id}/color")]
        //    public async Task<IActionResult> SetEventColor([FromRoute] int id, [FromBody] EventColorParams param)
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        var @event = await _context.Events.SingleOrDefaultAsync(m => m.Id == id);
        //        if (@event == null)
        //        {
        //            return NotFound();
        //        }

        //        @event.Color = param.Color;

        //        try
        //        {
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CalendarEventExists(id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }

        //        return NoContent();
        //    }
        //}


        //public class EventMoveParams
        //{
        //    public DateTime Start { get; set; }
        //    public DateTime End { get; set; }
        //}

        //public class EventColorParams
        //{
        //    public string Color { get; set; }
        //}
    }
}
