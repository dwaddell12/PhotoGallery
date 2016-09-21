namespace PhotoGallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedPhotoControllerAgain : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Photos", name: "Gallery_Id", newName: "Album_Id");
            AddColumn("dbo.Photos", "ByteContent", c => c.Binary());
            DropColumn("dbo.Photos", "Content");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Photos", "Content", c => c.Binary());
            DropColumn("dbo.Photos", "ByteContent");
            RenameColumn(table: "dbo.Photos", name: "Album_Id", newName: "Gallery_Id");
        }
    }
}
