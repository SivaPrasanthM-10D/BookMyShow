﻿using BookMyShow.Models;
using BookMyShow.Custom_Exceptions;
namespace BookMyShow.Implementations
{
    public static class AdminOperations
    {
        private static List<Movie> Movies = new List<Movie>();
        private static List<Screen> Screens = new List<Screen>();
        private static List<Theatre> Theatres = new List<Theatre>();
        private static Dictionary<string, double> Coupons = new Dictionary<string, double>();

        private static void WriteCentered(string text)
        {
            int windowWidth = 168;
            int textLength = text.Length;
            int spaces = (windowWidth - textLength) / 2;
            Console.WriteLine(new string(' ', spaces) + text);
        }

        public static void AddMovie(string title, string genre, int duration)
        {
            try
            {
                if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(genre) || duration <= 0)
                {
                    throw new ArgumentException("Invalid movie details provided.");
                }
                Movies.Add(new Movie(title, genre, duration));
                WriteCentered("Movie Added Successfully.");
            }
            catch (Exception ex)
            {
                WriteCentered($"Error: {ex.Message}");
            }
        }

        public static void AddScreen(int screenNumber)
        {
            try
            {
                if (screenNumber <= 0)
                {
                    throw new ArgumentException("Invalid screen number.");
                }
                Screens.Add(new Screen(screenNumber));
            }
            catch (Exception ex)
            {
                WriteCentered($"Error: {ex.Message}");
            }
        }

        public static void AddTheatre(string name, string city, string street, int numScreens)
        {
            try
            {
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(city) || string.IsNullOrEmpty(street) || numScreens <= 0)
                {
                    throw new ArgumentException("Invalid theatre details provided.");
                }
                Theatre theatre = new Theatre(name, city, street);
                for (int i = 1; i <= numScreens; i++)
                {
                    theatre.Screens.Add(new Screen(i));
                }
                Theatres.Add(theatre);
            }
            catch (Exception ex)
            {
                WriteCentered($"Error: {ex.Message}");
            }
        }

        public static List<Movie> GetMovies()
        {
            return Movies;
        }

        public static List<Theatre> GetTheatres()
        {
            return Theatres;
        }

        public static Dictionary<string, double> GetCoupons()
        {
            return Coupons;
        }

        public static bool ShowExists(string theatreName, int screenNo, DateTime showTime, DateTime showDate)
        {
            Theatre? theatre = Theatres.Find(t => t.Name.Equals(theatreName, StringComparison.OrdinalIgnoreCase));
            if (theatre == null)
            {
                throw new TheatreNotFoundException("Theatre not found.");
            }

            Screen? screen = theatre.Screens.Find(s => s.ScreenNumber == screenNo);
            if (screen == null)
            {
                throw new ScreenNotFoundException(screenNo, theatre.Name);
            }

            return screen.Shows.Any(s => s.ShowTime == showTime.ToString() && s.ShowDate == showDate.ToString());
        }

        public static void AddShow(string theatreName, int screenNo, string movieTitle, DateTime showTime, DateTime showDate, int availableSeats, double ticketPrice)
        {
            try
            {
                Movie? movie = Movies.Find(m => m.Title.Equals(movieTitle, StringComparison.OrdinalIgnoreCase));
                Theatre? theatre = Theatres.Find(t => t.Name.Equals(theatreName, StringComparison.OrdinalIgnoreCase));

                if (movie == null)
                {
                    throw new MovieNotFoundException("Movie not found.");
                }

                if (theatre == null)
                {
                    throw new TheatreNotFoundException("Theatre not found.");
                }

                Screen? screen = theatre.Screens.Find(s => s.ScreenNumber == screenNo);
                if (screen == null)
                {
                    throw new ScreenNotFoundException(screenNo, theatre.Name);
                }

                if (availableSeats < 0 || ticketPrice < 0)
                {
                    throw new ArgumentException("Invalid show details provided.");
                }

                screen.Shows.Add(new Show(movie, showTime, showDate, availableSeats, theatre, ticketPrice));
                WriteCentered($"Show successfully added: {movie.Title} at {theatre.Name}, Screen {screenNo}");
            }
            catch (Exception ex)
            {
                WriteCentered($"Error: {ex.Message}");
            }
        }

        public static void RemoveShow(string theatreName, int screenNo, string movieTitle, DateTime showTime)
        {
            try
            {
                Theatre? theatre = Theatres.Find(t => t.Name.Equals(theatreName, StringComparison.OrdinalIgnoreCase));
                if (theatre == null)
                {
                    throw new TheatreNotFoundException("Theatre not found.");
                }

                Screen? screen = theatre.Screens.Find(s => s.ScreenNumber == screenNo);
                if (screen == null)
                {
                    throw new ScreenNotFoundException(screenNo, theatre.Name);
                }

                Show? show = screen.Shows.Find(s => s.Movie.Title.Equals(movieTitle, StringComparison.OrdinalIgnoreCase) && s.ShowTime == showTime.ToString());
                if (show == null)
                {
                    throw new ShowNotFoundException("Show not found.");
                }

                screen.Shows.Remove(show);
                WriteCentered($"Show successfully removed: {movieTitle} at {theatre.Name}, Screen {screenNo}");
            }
            catch (Exception ex)
            {
                WriteCentered($"Error: {ex.Message}");
            }
        }

        public static void AddCoupon(string code, double discount)
        {
            try
            {
                if (string.IsNullOrEmpty(code) || discount <= 0 || discount > 100)
                {
                    throw new ArgumentException("Invalid coupon details provided.");
                }
                if (Coupons.ContainsKey(code))
                {
                    throw new DuplicateCouponException("Coupon already exists.");
                }
                Coupons[code] = discount;
                WriteCentered("Coupon added successfully!");
            }
            catch (Exception ex)
            {
                WriteCentered($"Error: {ex.Message}");
            }
        }
        public static void RemoveTheatre(string theatreName)
        {
            try
            {
                Theatre? theatre = Theatres.Find(t => t.Name.Equals(theatreName, StringComparison.OrdinalIgnoreCase));
                if (theatre == null)
                {
                    throw new TheatreNotFoundException("Theatre not found.");
                }

                Theatres.Remove(theatre);
                WriteCentered($"Theatre successfully removed: {theatreName}");
            }
            catch (Exception ex)
            {
                WriteCentered($"Error: {ex.Message}");
            }
        }

        public static void RemoveMovie(string movieTitle)
        {
            try
            {
                Movie? movie = Movies.Find(m => m.Title.Equals(movieTitle, StringComparison.OrdinalIgnoreCase));
                if (movie == null)
                {
                    throw new MovieNotFoundException("Movie not found.");
                }

                Movies.Remove(movie);
                WriteCentered($"Movie successfully removed: {movieTitle}");
            }
            catch (Exception ex)
            {
                WriteCentered($"Error: {ex.Message}");
            }
        }
    }
}
