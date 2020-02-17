/* Copyright (c) 2018, Oracle and/or its affiliates. All rights reserved. */
/* Copyright (c) .NET Foundation and Contributors                         */

/******************************************************************************
 *   Licensed under the Apache License, Version 2.0 (the "License");
 *   you may not use this file except in compliance with the License.
 *   You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *   Unless required by applicable law or agreed to in writing, software
 *   distributed under the License is distributed on an "AS IS" BASIS,
 *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *   limitations under the License.
 * 
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace OracleEFCore
{
	class Program
	{
		public class BloggingContext : DbContext
		{
			public DbSet<Blog> Blogs { get; set; }
			public DbSet<Post> Posts { get; set; }
			public DbSet<Paragraph> Paragraphs { get; set; }
			public DbSet<Word> Words { get; set; }

			protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			{
				optionsBuilder.UseOracle(@"User Id=C##BLOG;Password=blog;Data Source=localhost:1521/ORCLPDB1");
			}
		}

		public class Blog
		{
			public int BlogId { get; set; }
			public string Url { get; set; }
			public int Rating { get; set; }
			public List<Post> Posts { get; set; }
		}

		public class Post
		{
			public int PostId { get; set; }
			public string Title { get; set; }
			//public string Content { get; set; }
			public List<Paragraph> Paragraphs { get; set; }

			public int BlogId { get; set; }
			public Blog Blog { get; set; }
		}

		public class Paragraph
		{
			public int ParagraphId { get; set; }
			public string HtmlTag { get; set; }
			public List<Word> Words { get; set; }

			public int PostId { get; set; }
			public Post Post { get; set; }
		}

		public class Word
		{
			public int WordId { get; set; }
			public string Text { get; set; }

			public int ParagraphId { get; set; }
			public Paragraph Paragraph { get; set; }
		}

		static async Task Main(string[] args)
		{
			using (var db = new BloggingContext())
			{
				var all_blogs = await db.Blogs.Include(b => b.Posts).ToListAsync();
				if (all_blogs.Count <= 0)
				{
					var blog = new Blog
					{
						Url = "https://blogs.oracle.com" + " " + DateTime.Now.ToString(),
						Rating = 1,
						Posts = new List<Post>(new Post[] { new Post {
						Paragraphs = new List<Paragraph>(new Paragraph[]
						{
							new Paragraph
							{
								HtmlTag = "p",
								Words = new List<Word>( new Word[]
								{
									new Word { Text = "Lorem" },
									new Word { Text = "ipsum" },
									new Word { Text = "dolor" },
									new Word { Text = "sit" },
									new Word { Text = "amet" }
								})
							}
						}),
						Title = "Example title " + DateTime.Now.ToString()
					} })
					};
					db.Blogs.Add(blog);
					await db.SaveChangesAsync();
				}
				else
				{
					var blog = all_blogs.FirstOrDefault();
					var post = new Post
					{
						Paragraphs = new List<Paragraph>(new Paragraph[]
								{
									new Paragraph
									{
										HtmlTag = "p",
										Words = new List<Word>( new Word[]
										{
											new Word { Text = "Lorem updated" },
											new Word { Text = "ipsum updated" },
											new Word { Text = "dolor updated" },
											new Word { Text = "sit updated" },
											new Word { Text = "amet updated" }
										})
									}
								}),
						Title = "Example title " + DateTime.Now.ToString() + " updated",
						Blog = blog
					};


					using (var trans = await db.Database.BeginTransactionAsync())
					{
						try
						{
							blog.Posts.Add(post);
							await db.SaveChangesAsync();

							trans.Commit();
						}
						catch (Exception ex)
						{
							trans.Rollback();
							throw;
						}
					}
				}
			}

			using (var db = new BloggingContext())
			{
				var blogs = db.Blogs;
			}
		}
	}
}
