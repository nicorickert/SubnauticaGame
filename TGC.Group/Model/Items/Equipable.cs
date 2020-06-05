﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Items
{
    public enum EBodyPart
    {
        HEAD, BODY, LEGS, BACK, WEAPON
    }

    public class Equipable : Item
    {
        public override string ItemTypeDescription => "equipable";

        public EBodyPart BodyPart { get; private set; }

        public Equipable(EItemID id, string name, string spritePath, List<IItemEffect> onUseEffects, EBodyPart bodyPart)
            : base(id, name, spritePath, onUseEffects)
        {
            BodyPart = bodyPart;
        }

        public override void Use(Player user)
        {
            base.Use(user);
            user.Equipment.Equip(this);
        }
    }
}
