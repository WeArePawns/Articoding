/****************************************************************************

Functions for interpreting c# code for blocks.

Copyright 2016 sophieml1989@gmail.com

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

****************************************************************************/


using System.Collections;
using UnityEngine;

namespace UBlockly
{
    [CodeInterpreter(BlockType = "messages_instantiate_object")]
    public class Instantiate_Object_Cmdtor : EnumeratorCmdtor
    {
        protected override IEnumerator Execute(Block block)
        {
            Debug.Log("Message Sent");
            string value = block.GetFieldValue("N_OBJECTS");
            MessageManager.instance.sendMessage(value, MSG_TYPE.INSTANTIATE);

            yield return null;          
        }
    }

    [CodeInterpreter(BlockType = "messages_move_object")]
    public class Move_Object_Cmdtor : EnumeratorCmdtor
    {
        protected override IEnumerator Execute(Block block)
        {
            Debug.Log("Message Sent");
            string tag = block.GetFieldValue("TAG");
            string value = block.GetFieldValue("AMOUNT");
            MessageManager.instance.sendMessage(tag+ ' '+ value, MSG_TYPE.MOVE);

            yield return null;
        }
    }
}
