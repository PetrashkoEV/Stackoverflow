﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Stackoverflow.SqlContext.Entities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class StackoverflowEntities : DbContext
    {
        public StackoverflowEntities()
            : base("name=StackoverflowEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Answer> Answer { get; set; }
        public DbSet<CommentaryAnswer> CommentaryAnswer { get; set; }
        public DbSet<CommentaryQuestion> CommentaryQuestion { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<TemplateMessageEmail> TemplateMessageEmail { get; set; }
        public DbSet<User> User { get; set; }
    }
}