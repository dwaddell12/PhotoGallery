namespace PhotoGallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedPhotoController : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Photos", name: "PhotoGallery_Id", newName: "Gallery_Id");
            AddColumn("dbo.Photos", "Content", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Photos", "Content");
            RenameColumn(table: "dbo.Photos", name: "Gallery_Id", newName: "PhotoGallery_Id");
        }
    }
}
