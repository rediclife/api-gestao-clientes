﻿using Domain.Interfaces.Interfaces;
using Entities.Entities;
using Infrastructure.Configuration;
using Infrastructure.Repository.Generics;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Repositories
{
    public class RepositoryContato : RepositoyGenerics<Contato>, IContato
    {
        private readonly DbContextOptions<ContextBase> _OptionsBuilder;
        public RepositoryContato()
        {
            _OptionsBuilder = new DbContextOptions<ContextBase>();
        }
    }
}
