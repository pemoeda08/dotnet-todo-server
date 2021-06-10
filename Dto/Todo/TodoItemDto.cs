using System;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using TodoServer.Data.Entities;
using TodoServer.Dto.User;

namespace TodoServer.Dto.Todo
{

    [AutoMap(typeof(TodoItem), ReverseMap = true, PreserveReferences = true)]
    public class TodoItemDto
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }

        [SourceMember(nameof(TodoItem.CreatedById))]
        public int CreatedBy { get; set; }
    }

}