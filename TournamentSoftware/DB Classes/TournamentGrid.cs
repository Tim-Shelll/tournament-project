﻿using System;
using SQLite;

namespace TournamentSoftware
{
    [Table("TournamentGrid")]
    public class TournamentGrid
    {
        [Column("id"), PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("type")]
        public string Type { get; set; }
    }
}