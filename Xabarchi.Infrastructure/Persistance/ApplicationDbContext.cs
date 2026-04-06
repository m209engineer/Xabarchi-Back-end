using Microsoft.EntityFrameworkCore;
using Xabarchi.Domain.Model;

namespace Xabarchi.Infrastructure.Persistance;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Chatmember> Chatmembers { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Messagemedia> Messagemedia { get; set; }
    public DbSet<Reaction> Reactions { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Phone).IsUnique();
            
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Firstname).HasMaxLength(50);
            entity.Property(e => e.Lastname).HasMaxLength(50);
            entity.Property(e => e.Bio).HasMaxLength(500);
            entity.Property(e => e.AvatarUrl).HasMaxLength(500);
            
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        });
        
        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        });
        
        modelBuilder.Entity<Chatmember>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => new { e.ChatId, e.UserId }).IsUnique();
            
            entity.Property(e => e.JoinedAt).HasDefaultValueSql("NOW()");
            
            entity.HasOne(e => e.Chat)
                .WithMany(c => c.Members)
                .HasForeignKey(e => e.ChatId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.Chatmembers)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => e.ChatId);
            entity.HasIndex(e => e.SenderId);
            entity.HasIndex(e => new { e.ChatId, e.SentAt });
            
            entity.Property(e => e.Content).HasMaxLength(4000);
            entity.Property(e => e.SentAt).HasDefaultValueSql("NOW()");
            
            entity.HasOne(e => e.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(e => e.ChatId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(e => e.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.ReplyToMessage)
                .WithMany(m => m.Replies)
                .HasForeignKey(e => e.ReplyToMessageId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        modelBuilder.Entity<Messagemedia>(entity =>
        {
            entity.HasKey(e => e.Id);
    
            entity.HasIndex(e => e.MessageId);
    
            entity.Property(e => e.FileUrl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.PublicId).IsRequired().HasMaxLength(255);  
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.MimeType).HasMaxLength(100);
            entity.Property(e => e.ThumbnailUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
    
            entity.HasOne(e => e.Message)
                .WithMany(m => m.MediaFiles)
                .HasForeignKey(e => e.MessageId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<Reaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => new { e.UserId, e.MessageId, e.Content }).IsUnique();
            
            entity.Property(e => e.Content).IsRequired().HasMaxLength(10);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
            
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Message)
                .WithMany(m => m.Reactions)
                .HasForeignKey(e => e.MessageId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}