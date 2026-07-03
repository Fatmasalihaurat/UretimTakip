using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UretimTakip.core.Entities;

namespace UretimTakip.DataAccess.Interceptors
{
   public class AuditInterceptor : SaveChangesInterceptor
    {
        //Senkron için 
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            // Muhafız kontrolü: Eğer context boş değilse içeriği besle
            if (eventData.Context != null)
            {
                UpdateAuditProperties(eventData.Context);
            }
            return base.SavingChanges(eventData, result);
        }

        //Asenkron için
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            // Muhafız kontrolü: Eğer context boş değilse içeriği besle
            if (eventData.Context != null)
            {
                UpdateAuditProperties(eventData.Context);
            }
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        //Asıl işi yapan yardımcı method
        private  void UpdateAuditProperties(DbContext context)
        {
            if (context == null) return;
            //Değişiklik yakalama(ChangerTracker)
            var entries = context.ChangeTracker.Entries();
            foreach(var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("OlusturulmaTarihi").CurrentValue = DateTime.UtcNow;
                }

                else if (entry.State == EntityState.Modified)
                {
                    entry.Property("SonGuncellenmeTarihi").CurrentValue = DateTime.UtcNow;
                }
            }
        }
    }
}
