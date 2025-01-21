using C971_MobileApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C971_MobileApp
{
    internal static class DatabaseService
    {
        private static SQLiteAsyncConnection _database = null!;

        public static async Task InitializeAsync()
        {
            if (_database != null) return;

            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "C971.db");

            _database = new SQLiteAsyncConnection(databasePath);

                await _database.CreateTableAsync<Term>();
                await _database.CreateTableAsync<Course>();
                await _database.CreateTableAsync<Note>();

            // Seed default data
            await SeedDatabaseAsync();
        }

        public static SQLiteAsyncConnection Database => _database;

        // Add a Term to the database
        public static async Task AddTermAsync(Term term)
        {
            await _database.InsertAsync(term);
        }

        // Update an existing term
        public static async Task UpdateTermAsync(Term term)
        {
            await _database.UpdateAsync(term);
        }

        // Fetch all terms with their associated courses
        public static async Task<List<Term>> GetTermsWithCoursesAsync()
        {
            var terms = await _database.Table<Term>().ToListAsync();
            foreach (var term in terms)
            {
                term.Courses = await _database.Table<Course>().Where(c => c.TermID == term.ID).ToListAsync();
                term.IsExpanded = false; // Default to collapsed

                //Debug
                Console.WriteLine($"Term: {term.Title}, Courses Count: {term.Courses.Count}");
            }
            return terms;
        }

        // Delete a term and its associated courses
        public static async Task DeleteTermWithCoursesAsync(Term term)
        {
            await _database.DeleteAsync(term);
            var courses = await _database.Table<Course>().Where(c => c.TermID == term.ID).ToListAsync();
            foreach (var course in courses)
            {
                await _database.DeleteAsync(course);
            }
        }

        // Get notes by course ID
        public static async Task<List<Note>> GetNotesByCourseIdAsync(int courseId)
        {
            return await _database.Table<Note>()
                .Where(note => note.CourseId == courseId)
                .ToListAsync();
        }

        // Add note to the database
        public static async Task AddNoteAsync(Note note)
        {
            await _database.InsertAsync(note);
        }

        // Update note in database
        public static async Task UpdateNoteAsync(Note note)
        {
            await _database.UpdateAsync(note);
        }

        // Delete note from database
        public static async Task DeleteNoteAsync(Note note)
        {
            await _database.DeleteAsync(note);
        }

        // Task to create default data in the database
        private static async Task SeedDatabaseAsync()
        {
            // Check if there are any terms in the database
            var terms = await _database.Table<Term>().ToListAsync();
            if (terms.Count == 0)
            {
                // Create Term 1 (past term)
                var term1 = new Term
                {
                    Title = "Summer 2024",
                    StartDate = new DateTime(2024, 5, 1),
                    EndDate = new DateTime(2024, 10, 31)
                };
                // Create Term 2 (current term)
                var term2 = new Term
                {
                    Title = "Fall 2024",
                    StartDate = new DateTime(2024, 11, 1),
                    EndDate = new DateTime(2025, 4, 30)
                };

                await _database.InsertAsync(term1);
                await _database.InsertAsync(term2);

                // Generate courses for each term
                await GenerateCourses(term1.ID, term2.ID);
            }
        }

        private static async Task GenerateCourses(int Term1ID, int Term2ID)
        {
            var courses = new List<Course>();

            // Courses for Term 1
            courses.AddRange(new List<Course>
                {
                new Course { TermID = Term1ID, Title = "Math 101", Status = "Passed", StartDate = new DateTime(2024, 5, 1), EndDate = new DateTime(2024, 6, 30) },
                new Course { TermID = Term1ID, Title = "English 101", Status = "Passed", StartDate = new DateTime(2024, 7, 1), EndDate = new DateTime(2024, 8, 31) },
                new Course { TermID = Term1ID, Title = "History 101", Status = "Passed", StartDate = new DateTime(2024, 9, 1), EndDate = new DateTime(2024, 9, 30) },
                new Course { TermID = Term1ID, Title = "Science 101", Status = "Passed", StartDate = new DateTime(2024, 10, 1), EndDate = new DateTime(2024, 10, 31) },
                new Course { TermID = Term1ID, Title = "Art 101", Status = "Passed", StartDate = new DateTime(2024, 5, 1), EndDate = new DateTime(2024, 6, 30) },
                new Course { TermID = Term1ID, Title = "Computer Science 101", Status = "Not Passed", StartDate = new DateTime(2024, 7, 1), EndDate = new DateTime(2024, 8, 31) }
                });

            // Courses for Term 2
            courses.AddRange(new List<Course>
            {
                new Course { TermID = Term2ID, Title = "Math 201", Status = "Passed", StartDate = new DateTime(2024, 11, 1), EndDate = new DateTime(2024, 11, 30) },
                new Course { TermID = Term2ID, Title = "English 201", Status = "Passed", StartDate = new DateTime(2024, 12, 1), EndDate = new DateTime(2024, 12, 31) },
                new Course { TermID = Term2ID, Title = "History 201", Status = "Started", StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 1, 31) },
                new Course { TermID = Term2ID, Title = "Science 201", Status = "Enrolled", StartDate = new DateTime(2025, 2, 1), EndDate = new DateTime(2025, 2, 28) },
                new Course { TermID = Term2ID, Title = "Art 201", Status = "Enrolled", StartDate = new DateTime(2025, 3, 1), EndDate = new DateTime(2025, 3, 31) },
                new Course { TermID = Term2ID, Title = "Computer Science 101", Status = "Enrolled", StartDate = new DateTime(2025, 4, 1), EndDate = new DateTime(2025, 4, 30) }
            });

            // Insert courses into the database
            foreach (var course in courses)
            {
                await _database.InsertAsync(course);
            }

        }
    }

}
