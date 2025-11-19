using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SIGIT.Models;
using SIGIT.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SIGIT.Controllers
{
    [Authorize]
    public class EmpleadosController : Controller
    {
        private readonly SigitContext _context;
        private readonly IConfiguration _config;

        public EmpleadosController(SigitContext context, IConfiguration config)
        {
            _context = context;
            _config = config; // Inyección de la configuración para leer EmailSettings
        }

        // GET: Empleados
        public async Task<IActionResult> Index()
        {
            var sigitContext = _context.Empleados.Include(e => e.Area).Include(e => e.Cargo).Include(e => e.Ciudad).Include(e => e.Compania).Include(e => e.Estatus);
            return View(await sigitContext.ToListAsync());
        }

        // Listado de la pagina inicial de empleados
        // GET: Empleados/Listado
        public async Task<IActionResult> Listado()
        {
            var empleadosDesdeBD = await _context.Empleados
                .Include(e => e.Area)
                .Include(e => e.Cargo)
                .Include(e => e.Ciudad)
                .Include(e => e.Estatus)
                .ToListAsync();

            var empleadosParaMostrar = empleadosDesdeBD.Select(e => new EmpleadoListadoViewModel
            {
                Id = e.EmpleadoId,
                NombreCompleto = $"{e.Nombre} {e.SegundoNombre} {e.Apellido} {e.SegundoApellido}".Trim().Replace("  ", " "),
                Celular = e.Celular,
                Area = e.Area.NombreArea,
                Cargo = e.Cargo.NombreCargo,
                Ciudad = e.Ciudad.NombreCiudad,
                Estatus = e.Estatus.NombreEstatus
            }).ToList();

            return View(empleadosParaMostrar);
        }

        // GET: Empleados/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados
                .Include(e => e.Area)
                .Include(e => e.Cargo)
                .Include(e => e.Ciudad)
                .Include(e => e.Compania)
                .Include(e => e.Estatus)
                .Include(e => e.CuentasEmpleados)
                .ThenInclude(cuenta => cuenta.Aplicacion)
                .FirstOrDefaultAsync(m => m.EmpleadoId == id);

            if (empleado == null)
            {
                return NotFound();
            }

            return PartialView(empleado);
        }

        // GET: Empleados/Create
        [Authorize(Roles = "Administrador, Técnico")]
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return PartialView();
        }

        // POST: Empleados/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Técnico")]
        public async Task<IActionResult> Create([Bind("EmpleadoId,Cedula,Nombre,SegundoNombre,Apellido,SegundoApellido,Celular,CorreoPersonal,CargoId,AreaId,CiudadId,CompaniaId,EstatusId,FechaIngreso,FechaRetiro")] Empleado empleado)
        {
            empleado.FechaRegistro = DateTime.Now;
            ModelState.Remove("FechaRegistro");
            ModelState.Remove("Area");
            ModelState.Remove("Cargo");
            ModelState.Remove("Ciudad");
            ModelState.Remove("Compania");
            ModelState.Remove("Estatus");

            if (ModelState.IsValid)
            {
                _context.Add(empleado);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            await PopulateDropdowns(empleado);
            return PartialView(empleado);
        }

        // GET: Empleados/Edit/5
        [Authorize(Roles = "Administrador, Técnico")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }

            await PopulateDropdowns(empleado);
            return PartialView(empleado);
        }

        // POST: Empleados/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador, Técnico")]
        public async Task<IActionResult> Edit(int id, [Bind("EmpleadoId,Cedula,Nombre,SegundoNombre,Apellido,SegundoApellido,Celular,CorreoPersonal,CargoId,AreaId,CiudadId,CompaniaId,EstatusId,FechaIngreso,FechaRetiro")] Empleado empleado)
        {
            if (id != empleado.EmpleadoId)
            {
                return NotFound();
            }

            ModelState.Remove("FechaRegistro");
            ModelState.Remove("Area");
            ModelState.Remove("Cargo");
            ModelState.Remove("Ciudad");
            ModelState.Remove("Compania");
            ModelState.Remove("Estatus");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empleado);
                    _context.Entry(empleado).Property(x => x.FechaRegistro).IsModified = false;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpleadoExists(empleado.EmpleadoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Json(new { success = true });
            }

            await PopulateDropdowns(empleado);
            return PartialView(empleado);
        }

        // GET: Empleados/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado = await _context.Empleados
                .Include(e => e.Area)
                .Include(e => e.Cargo)
                .Include(e => e.Ciudad)
                .Include(e => e.Compania)
                .Include(e => e.Estatus)
                .FirstOrDefaultAsync(m => m.EmpleadoId == id);

            if (empleado == null)
            {
                return NotFound();
            }

            return PartialView(empleado);
        }

        // POST: Empleados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado != null)
            {
                _context.Empleados.Remove(empleado);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Listado));
        }

        private bool EmpleadoExists(int id)
        {
            return _context.Empleados.Any(e => e.EmpleadoId == id);
        }

        // --- MÉTODO AUXILIAR PARA DROPDOWNS ---
        private async Task PopulateDropdowns(Empleado? empleado = null)
        {
            if (empleado == null)
            {
                ViewBag.AreaId = new SelectList(await _context.Areas.ToListAsync(), "AreaId", "NombreArea");
                ViewBag.CargoId = new SelectList(await _context.Cargos.ToListAsync(), "CargoId", "NombreCargo");
                ViewBag.CiudadId = new SelectList(await _context.Ciudades.ToListAsync(), "CiudadId", "NombreCiudad");
                ViewBag.CompaniaId = new SelectList(await _context.Companias.ToListAsync(), "CompaniaId", "NombreCompania");
                ViewBag.EstatusId = new SelectList(await _context.Estatuses.ToListAsync(), "EstatusId", "NombreEstatus");
            }
            else
            {
                ViewBag.AreaId = new SelectList(await _context.Areas.ToListAsync(), "AreaId", "NombreArea", empleado.AreaId);
                ViewBag.CargoId = new SelectList(await _context.Cargos.ToListAsync(), "CargoId", "NombreCargo", empleado.CargoId);
                ViewBag.CiudadId = new SelectList(await _context.Ciudades.ToListAsync(), "CiudadId", "NombreCiudad", empleado.CiudadId);
                ViewBag.CompaniaId = new SelectList(await _context.Companias.ToListAsync(), "CompaniaId", "NombreCompania", empleado.CompaniaId);
                ViewBag.EstatusId = new SelectList(await _context.Estatuses.ToListAsync(), "EstatusId", "NombreEstatus", empleado.EstatusId);
            }
        }

        // ACCIÓN DE ENVÍO DE CORREO DESDE DETALLES
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnviarNotificacion(int id)
        {
            // Obtenemos los datos del empleado (incluyendo cuentas para el correo)
            var empleado = await _context.Empleados
                .Include(e => e.Area)
                .Include(e => e.Cargo)
                .Include(e => e.Ciudad)
                .Include(e => e.Compania)
                .Include(e => e.Estatus)
                .Include(e => e.CuentasEmpleados)
                .ThenInclude(c => c.Aplicacion)
                .FirstOrDefaultAsync(m => m.EmpleadoId == id);

            if (empleado == null)
            {
                return Json(new { success = false, message = "Empleado no encontrado." });
            }

            try
            {
                // Leemos la configuración del appsettings.json
                var emailConfig = _config.GetSection("EmailSettings");
                string smtpServer = emailConfig["SmtpServer"];
                int smtpPort = int.Parse(emailConfig["SmtpPort"]);
                string senderName = emailConfig["SenderName"];
                string senderEmail = emailConfig["SenderEmail"];
                string senderPassword = emailConfig["SenderPassword"];

                // Construcción del cuerpo del correo
                var nombreCompleto = string.Join(" ", new[] { empleado.Nombre, empleado.SegundoNombre, empleado.Apellido, empleado.SegundoApellido }
                    .Where(s => !string.IsNullOrWhiteSpace(s)));

                var bodyBuilder = new System.Text.StringBuilder();
                bodyBuilder.AppendLine($"Cordial saludo, {nombreCompleto}");
                bodyBuilder.AppendLine();
                bodyBuilder.AppendLine("Remito los datos de conexión para las aplicaciones de Autofinanciera a las cuales tiene acceso.");
                bodyBuilder.AppendLine();
                bodyBuilder.AppendLine("URL SIICON: http://gestion.siicon.com.co");
                bodyBuilder.AppendLine("URL Qurii: http://qurii.co");
                bodyBuilder.AppendLine("URL HelpDesk: http://helpdesk.serven.com.co:8080/");
                bodyBuilder.AppendLine();
                bodyBuilder.AppendLine("Se otorgará solamente acceso a los tableros a las cuentas corporativas.");
                bodyBuilder.AppendLine("Tenga presente que los usuarios y contraseñas son personales e intransferibles, abstengase de compartir esta información ya que esto va en contra de las normas de seguridad de la información.");
                bodyBuilder.AppendLine();
                bodyBuilder.AppendLine("DATOS DEL USUARIO PARA NOTIFICACIONES ---");
                bodyBuilder.AppendLine();
                bodyBuilder.AppendLine($"Correo de notificaciones: {empleado.CorreoPersonal}");
                bodyBuilder.AppendLine($"Celular: {empleado.Celular}");
                bodyBuilder.AppendLine($"Área: {empleado.Area?.NombreArea}");
                bodyBuilder.AppendLine($"Ciudad: {empleado.Ciudad?.NombreCiudad}");
                bodyBuilder.AppendLine($"Compañía: {empleado.Compania?.NombreCompania}");
                bodyBuilder.AppendLine();
                bodyBuilder.AppendLine("--- CUENTAS ASIGNADAS ---");

                foreach (var cuenta in empleado.CuentasEmpleados)
                {
                    bodyBuilder.AppendLine($"Aplicación: {cuenta.Aplicacion?.NombreAplicacion}");
                    bodyBuilder.AppendLine($"Usuario: {cuenta.Usuario}");
                    bodyBuilder.AppendLine($"Contraseña: {cuenta.Contrasena}");
                    bodyBuilder.AppendLine();
                }

                string emailBody = bodyBuilder.ToString();

                // Configuramos el cliente SMTP y el mensaje
                var client = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true // Requerido por Gmail
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = "Credenciales acceso aplicaciones Autofinanciera - Fonbienes",
                    Body = emailBody,
                    IsBodyHtml = false
                };
                mailMessage.To.Add(empleado.CorreoPersonal);

                // Envío del correo
                await client.SendMailAsync(mailMessage);

                // Devuelve con éxito al modal
                return Json(new { success = true, message = "Correo enviado exitosamente." });
            }
            catch (Exception ex)
            {
                // Si algo falla (firewall, credenciales, etc.) devolvemos el error
                return Json(new { success = false, message = $"Error al enviar el correo: {ex.Message}" });
            }
        }

        // RECUPERAR CREDENCIALES (Modo Anónimo)
        // Permite que un usuario NO logueado pida sus credenciales por cédula
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous] // Permite el acceso sin iniciar sesión
        public async Task<IActionResult> RecuperarCredenciales(string cedula)
        {
            // Busco al empleado por cédula
            var empleado = await _context.Empleados
                .Include(e => e.CuentasEmpleados)
                    .ThenInclude(ce => ce.Aplicacion)
                .Include(e => e.Area)
                .Include(e => e.Ciudad)
                .Include(e => e.Compania)
                .Include(e => e.Estatus)
                .FirstOrDefaultAsync(e => e.Cedula == cedula);

            if (empleado == null)
            {
                // Mensaje vago por seguridad: no decimos si la cédula existe o no.
                return Json(new { success = false, message = "Procesando solicitud. Si la cédula existe, el correo será enviado." });
            }

            try
            {
                // Reutilizamos la lógica de envío de correo (la movemos aquí para no duplicar código)
                var emailConfig = _config.GetSection("EmailSettings");
                string smtpServer = emailConfig["SmtpServer"];
                int smtpPort = int.Parse(emailConfig["SmtpPort"]);
                string senderName = emailConfig["SenderName"];
                string senderEmail = emailConfig["SenderEmail"];
                string senderPassword = emailConfig["SenderPassword"];

                // Construcción del cuerpo del correo
                var nombreCompleto = string.Join(" ", new[] { empleado.Nombre, empleado.SegundoNombre, empleado.Apellido, empleado.SegundoApellido }
                    .Where(s => !string.IsNullOrWhiteSpace(s)));

                var bodyBuilder = new System.Text.StringBuilder();
                bodyBuilder.AppendLine($"Cordial saludo, {nombreCompleto}");
                bodyBuilder.AppendLine();
                bodyBuilder.AppendLine("Remito los datos de conexión para las aplicaciones de Autofinanciera y/o Fonbienes a las cuales tiene acceso.");
                bodyBuilder.AppendLine();
                bodyBuilder.AppendLine("URL SIICON: http://gestion.siicon.com.co");
                bodyBuilder.AppendLine("URL Qurii: http://qurii.co");
                bodyBuilder.AppendLine("URL HelpDesk: http://helpdesk.serven.com.co:8080/");
                bodyBuilder.AppendLine();
                bodyBuilder.AppendLine("Se otorgará solamente acceso a los tableros a las cuentas corporativas.");
                bodyBuilder.AppendLine("Tenga presente que los usuarios y contraseñas son personales e intransferibles, abstengase de compartir esta información ya que esto va en contra de las normas de seguridad de la información.");
                bodyBuilder.AppendLine();
                bodyBuilder.AppendLine("DATOS DEL USUARIO PARA NOTIFICACIONES");
                bodyBuilder.AppendLine();
                bodyBuilder.AppendLine($"Correo de notificaciones: {empleado.CorreoPersonal}");
                bodyBuilder.AppendLine($"Celular: {empleado.Celular}");
                bodyBuilder.AppendLine($"Área: {empleado.Area?.NombreArea}");
                bodyBuilder.AppendLine($"Ciudad: {empleado.Ciudad?.NombreCiudad}");
                bodyBuilder.AppendLine($"Compañía: {empleado.Compania?.NombreCompania}");
                bodyBuilder.AppendLine();
                bodyBuilder.AppendLine("CUENTAS ASIGNADAS:");

                foreach (var cuenta in empleado.CuentasEmpleados)
                {
                    bodyBuilder.AppendLine($"Aplicación: {cuenta.Aplicacion?.NombreAplicacion}");
                    bodyBuilder.AppendLine($"Usuario: {cuenta.Usuario}");
                    bodyBuilder.AppendLine($"Contraseña: {cuenta.Contrasena}");
                    bodyBuilder.AppendLine();
                }

                string emailBody = bodyBuilder.ToString();

                // Configuramos el cliente SMTP y el mensaje
                var client = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true // Requerido por Gmail
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = "Credenciales acceso aplicaciones Autofinanciera - Fonbienes",
                    Body = emailBody,
                    IsBodyHtml = false
                };
                mailMessage.To.Add(empleado.CorreoPersonal);

                // Envío del correo
                await client.SendMailAsync(mailMessage);


                // Respuesta de éxito (Vaga por seguridad, para que el atacante no sepa si la cédula es real o no)
                return Json(new { success = true, message = "Procesando solicitud. Si la cédula es correcta, el correo será enviado a tu cuenta personal." });
            }
            catch (Exception)
            {
                // Si el envío falla por cualquier razón (ej. SMTP no conecta), 
                // devolvemos el mismo mensaje de éxito vago para no dar pistas al atacante.
                return Json(new { success = true, message = "Procesando solicitud. Si la cédula es correcta, el correo será enviado a tu cuenta personal." });
            }
        }
    }
}