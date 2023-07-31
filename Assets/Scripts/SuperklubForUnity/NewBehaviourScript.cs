using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Superklub;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SupersynkClientDTO dto = new SupersynkClientDTO("ada");
        dto.AddProperty("titi", "pos=1,2,3");
        string s0 = dto.ToJSONString();
        Debug.Log("s0=" + s0);

        string s1 = "{\"list\":[" + s0 + "]}";

        SupersynkClientDTOs dtos = new SupersynkClientDTOs();
        string s2 = "{\"list\":[{\"client_id\":\"ada\", \"properties\":[{\"key\":\"titi\",\"value\":\"pos=1,2,3\"}]}]}";

        Debug.Log("s1=" + s1);
        Debug.Log("s2=" + s2);

        string s3 = "[" + s0 + "]";

        dtos.FromJSONString(s3);

        Debug.Log(dtos.List.Count);
        var dto0 = dtos.List[0];
        Debug.Log(dto0.ClientId);
        Debug.Log(dto0.Properties.Count);
        Debug.Log(dto0.Properties[0].Key);
        Debug.Log(dto0.Properties[0].Value);

        var nodes = SuperklubNodeConverter.ConvertFromSupersynk(dto0);
        Debug.Log(nodes.Count);
        Debug.Log(nodes[0].Position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
