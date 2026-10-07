using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using POOI_T2_PANANA_CHUMPITAZ.Models;

namespace POOI_T2_PANANA_CHUMPITAZ.Controllers
{
    public class AlumnoController : Controller
    {
        // Ruta del archivo JSON
        private string rutaArchivo = "~/App_Data/alumnos.json";

        // Método para obtener la colección desde el archivo JSON
        private List<Alumno> ObtenerAlumnos()
        {
            string ruta = Server.MapPath(rutaArchivo);

            if (!System.IO.File.Exists(ruta))
            {
                return new List<Alumno>();
            }

            string contenido = System.IO.File.ReadAllText(ruta);

            if (string.IsNullOrWhiteSpace(contenido))
            {
                return new List<Alumno>();
            }

            return JsonConvert.DeserializeObject<List<Alumno>>(contenido)
                   ?? new List<Alumno>();
        }

        // Método para serializar y guardar la colección
        private void GuardarAlumnos(List<Alumno> alumnos)
        {
            string ruta = Server.MapPath(rutaArchivo);

            string json = JsonConvert.SerializeObject(
                alumnos,
                Formatting.Indented
            );

            System.IO.File.WriteAllText(ruta, json);
        }

        // GET: Alumno
        public ActionResult Index()
        {
            List<Alumno> alumnos = ObtenerAlumnos();

            return View(alumnos);
        }

        // GET: Alumno/Agregar
        public ActionResult Agregar()
        {
            return View();
        }

        // POST: Alumno/Agregar
        [HttpPost]
        public ActionResult Agregar(Alumno alumno)
        {
            List<Alumno> alumnos = ObtenerAlumnos();

            // Verificar que el DNI no esté repetido
            if (alumnos.Any(a => a.dni == alumno.dni))
            {
                ViewBag.Mensaje = "El DNI ingresado ya existe.";
                return View(alumno);
            }

            alumnos.Add(alumno);

            GuardarAlumnos(alumnos);

            TempData["Mensaje"] = "Alumno registrado correctamente.";

            return RedirectToAction("Index");
        }

        // GET: Alumno/Eliminar
        public ActionResult Eliminar(string dni)
        {
            List<Alumno> alumnos = ObtenerAlumnos();

            Alumno alumno = alumnos.FirstOrDefault(a => a.dni == dni);

            if (alumno != null)
            {
                alumnos.Remove(alumno);

                GuardarAlumnos(alumnos);

                TempData["Mensaje"] = "Alumno eliminado correctamente.";
            }

            return RedirectToAction("Index");
        }

        // GET: Alumno/Detalles
        public ActionResult Detalles(string dni)
        {
            List<Alumno> alumnos = ObtenerAlumnos();

            Alumno alumno = alumnos.FirstOrDefault(a => a.dni == dni);

            if (alumno == null)
            {
                return RedirectToAction("Index");
            }

            return View(alumno);
        }

        // GET: Alumno/Actualizar
        public ActionResult Actualizar(string dni)
        {
            List<Alumno> alumnos = ObtenerAlumnos();

            Alumno alumno = alumnos.FirstOrDefault(a => a.dni == dni);

            if (alumno == null)
            {
                return RedirectToAction("Index");
            }

            return View(alumno);
        }

        // POST: Alumno/Actualizar
        [HttpPost]
        public ActionResult Actualizar(Alumno alumno, string dniOriginal)
        {
            List<Alumno> alumnos = ObtenerAlumnos();

            // Verificar que el DNI no pertenezca a otro alumno
            if (alumnos.Any(a => a.dni == alumno.dni &&
                                 a.dni != dniOriginal))
            {
                ViewBag.Mensaje = "El DNI ingresado ya pertenece a otro alumno.";

                return View(alumno);
            }

            // Buscar al alumno por su DNI original
            Alumno alumnoExistente =
                alumnos.FirstOrDefault(a => a.dni == dniOriginal);

            if (alumnoExistente == null)
            {
                return RedirectToAction("Index");
            }

            // Actualizar los datos
            alumnoExistente.dni = alumno.dni;
            alumnoExistente.nombres = alumno.nombres;
            alumnoExistente.apellidos = alumno.apellidos;
            alumnoExistente.carrera = alumno.carrera;
            alumnoExistente.ciclo = alumno.ciclo;

            // Serializar nuevamente la colección
            GuardarAlumnos(alumnos);

            TempData["Mensaje"] = "Alumno actualizado correctamente.";

            return RedirectToAction("Index");
        }
    }
}