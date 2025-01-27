using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C971_MobileApp.Models
{
    internal static class DatabaseService
    {
        private static SQLiteAsyncConnection _database = null!;

        public static async Task InitializeAsync()
        {
            if (_database == null)
            {
                string dbPath = Path.Combine(FileSystem.AppDataDirectory, "C971.db");
                _database = new SQLiteAsyncConnection(dbPath);
            }

            await _database.CreateTableAsync<Term>();
            await _database.CreateTableAsync<Course>();
            await _database.CreateTableAsync<Note>();
            await _database.CreateTableAsync<Assessment>();

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
            // Check if terms already exist in the database
            var terms = await _database.Table<Term>().ToListAsync();
            if (terms.Count > 0) return; // Skip seeding if data already exists

            // Create one term
            var term = new Term
            {
                Title = "Spring 2025",
                StartDate = DateTime.Now.AddDays(1),
                EndDate = new DateTime(2025, 6, 30)
            };
            await _database.InsertAsync(term);

            // Create one course with provided instructor information
            var course = new Course
            {
                TermID = term.ID,
                Title = "Mobile Programming",
                Status = "Started",
                StartDate = DateTime.Now.AddDays(1),
                EndDate = new DateTime(2025, 6, 30),
                InstructorName = "Anika Patel",
                InstructorPhone = "555-123-4567",
                InstructorEmail = "anika.patel@strimeuniversity.edu"
            };
            await _database.InsertAsync(course);

            // Create two assessments for the course
            var assessments = new List<Assessment>
            {
                new Assessment
                {
                    CourseID = course.ID,
                    Type = "Performance",
                    Title = "Final Project",
                    StartDate = DateTime.Now.AddDays(1),
                    EndDate = new DateTime(2025, 5, 31),
                    StartDateNotificationEnabled = false,
                    EndDateNotificationEnabled = false
                },
                new Assessment
                {
                    CourseID = course.ID,
                    Type = "Objective",
                    Title = "Midterm Exam",
                    StartDate = DateTime.Now.AddDays(5),
                    EndDate = DateTime.Now.AddDays(5),
                    StartDateNotificationEnabled = false,
                    EndDateNotificationEnabled = false
                }
            };

            foreach (var assessment in assessments)
            {
                await _database.InsertAsync(assessment);
            }

        }
        // Add an assessment
        public static async Task AddAssessmentAsync(Assessment assessment)
        {
            try
            {
                await _database.InsertAsync(assessment);
                Console.WriteLine("Assessment added successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding assessment: {ex.Message}");
            }
        }

        // Update an assessment
        public static Task UpdateAssessmentAsync(Assessment assessment)
        {
            return _database.UpdateAsync(assessment);
        }

        // Delete an assessment
        public static async Task DeleteAssessmentAsync(Assessment assessment)
        {
            await _database.DeleteAsync(assessment);
        }

    }

}
