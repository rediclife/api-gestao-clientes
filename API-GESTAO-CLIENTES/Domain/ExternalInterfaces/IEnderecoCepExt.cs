using Entities.ExternalEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ExternalInterfaces
{
    public interface IEnderecoCepExt
    {
        EnderecoCepExt Consultar(string cep);
    }
}
