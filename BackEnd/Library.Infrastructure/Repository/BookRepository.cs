﻿using AutoMapper;
using Library.Application.DTOs.AuthorDTOs;
using Library.Application.DTOs.BookDTOs;
using Library.Application.RepositoryInterfaces;
using Library.Domain.Entity;
using Library.Domain.Enums;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Infrastructure.Repository
{
    public class BookRepository(ApplicationDbContext context) : IBookRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Book> AddBook(Book book)
        {
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<Book> DeleteBook(Book book)
        {
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<Book> EditBook(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<List<Book>> GetAllBooks()
        {
            var bookList = await _context.Books.Include(b => b.Authors).ToListAsync();
            return bookList;
        }



        public async Task<Book> GetBookById(int id)
        {
            var bookById = await _context.Books.Include(b => b.Authors).FirstOrDefaultAsync(b => b.BookId == id);
            return bookById;
        }

        public async Task<Book> GetBookByTitle(string title)
        {
            var bookByTitle = await _context.Books.Include(b => b.Authors).FirstOrDefaultAsync(a => a.Title == title);
            return bookByTitle;
        }

        public async Task<List<Book>> GetBooksByLanguage(string language)
        {
            if (!Enum.TryParse<Languages>(language, true, out var selectedLanguage))
                return new List<Book>();


            var bookList = await _context.Books
                .Include(b => b.Authors)
                .Where(b => b.Languages == selectedLanguage)
                .ToListAsync();

            return bookList;
        }
        public async Task<List<Book>> GetBooksByGenre(string genre)
        {
            if (!Enum.TryParse<GenreTypes>(genre, true, out var selectedGenre))
                return new List<Book>();

            var bookList = await _context.Books
                .Include(b => b.Authors)
                .Where(b => b.Genre == selectedGenre)
                .ToListAsync();

            return bookList;
        }
    }
}

