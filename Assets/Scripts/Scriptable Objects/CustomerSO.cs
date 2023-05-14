using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GenderEnum {
    None,
    Male,
    Female
}

[CreateAssetMenu(menuName = "Customer", fileName = "New Customer")]
public class CustomerSO : ScriptableObject {
    public string customerName;
    public GenderEnum customerGender;
    public List<GoodsSO> produces;  // list of goods that this customer produces
    public List<GoodsSO> accepts;   // list of goods that this customer accepts

    public bool Accepts(GoodsSO goods) {
        return accepts.Count > 0 && accepts.Contains(goods);
    }

    public bool Produces(GoodsSO goods) {
        return produces.Count > 0 && produces.Contains(goods);
    }
}
