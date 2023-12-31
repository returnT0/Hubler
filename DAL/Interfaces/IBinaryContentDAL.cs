﻿using Hubler.DAL.Models;

namespace Hubler.DAL.Interfaces;

public interface IBinaryContentDAL
{
    BinaryContent GetById(int? id);
    int Insert(BinaryContent content);
    void Update(BinaryContent content);
    void Delete(int? id);
    List<BinaryContent> GetAll();
}