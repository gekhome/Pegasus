using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Web;
using Pegasus.DAL;


namespace Pegasus.BPM
{
    public static class Commit
    {
        public static string CommitData(PegasusDBEntities db)
        {
            string emsg = "";
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException cex)
            {
                emsg += cex.Message;
                // delay N secs and try again
                delay(db, 2);
                return emsg;
            }
            catch (Exception ex)
            {
                emsg = "Προέκυψε γενικό σφάλμα κατά την αποθήκευση: " + "\n";
                emsg += ex.Message + "\n";
                emsg += "Επιστρέψτε στην προηγούμενη σελίδα και δοκιμάστε πάλι.";
            }
            return emsg;
        }

        private static void delay(PegasusDBEntities db, int seconds)
        {
            System.Timers.Timer delayTimer;

            delayTimer = new System.Timers.Timer();
            delayTimer.Interval = seconds * 1000;
            delayTimer.Elapsed += (o, e) => db.SaveChanges();
            delayTimer.Start();
        }

        private static void _delayTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // do nothing
        }
    }
}