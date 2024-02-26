using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DNA_CAPI_MIS.Models
{
    public class Translation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        //Entity can be Database table or Form or Page
        [Required]
        [Column(TypeName = "varchar")]
        [Display(Name = "Entity Name")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Entity name cannot be longer than 30 characters.")]
        public string EntityName { get; set; }

        //Primary key value of the table record OR '0' in case entity is a Form/Page
        [Required]
        [Column(TypeName = "varchar")]
        [Display(Name = "Key Value")]
        [StringLength(80, MinimumLength = 1, ErrorMessage = "Key Value cannot be longer than 80 characters.")]
        public string KeyValue { get; set; }

        //Field Name will contain Field Name of Database Table or Prompt Name/Code of Form/Page
        [Required]
        [Column(TypeName = "varchar")]
        [Display(Name = "Field Name")]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Field name cannot be longer than 30 characters.")]
        public string FieldName { get; set; }

        //Primary key value of the table record OR '0' in case entity is a Form/Page
        [Required]
        [Column(TypeName = "varchar")]
        [Display(Name = "Language")]
        [StringLength(6, MinimumLength = 5, ErrorMessage = "Language cannot be longer than 6 characters.")]
        public string Language { get; set; }

        [Column(TypeName = "ntext")]
        [MaxLength]
        [StringLength(2048, MinimumLength = 1, ErrorMessage = "Translated text is too large.")]
        public string Text { get; set; }


    }
}