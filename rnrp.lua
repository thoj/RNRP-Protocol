-- Redundant Network Routing Protocol (rnrp) dissector for Wireshark
-- Copyright Thomas Jäger 2017
-- https://github.com/thoj
-- License: https://tldrlegal.com/license/bsd-3-clause-license-(revised)


p_rnrp = Proto ("rnrp","Redundant Network Routing Protocol")
local f_path = ProtoField.uint8("rnrp.path", "Path Number", base.DEC)
local f_nodenumber = ProtoField.uint16("rnrp.node", "Node Number", base.DEC)
local f_area = ProtoField.uint8("rnrp.area", "Network Area", base.DEC)
local f_counter = ProtoField.uint16("rnrp.counter", "Counter", base.DEC)
--local f_networknumber = ProtoField.uint16("rnrp.networkarea", "Network Area", base.HEX)
--local f_data = ProtoField.string("rnrå.data", "Data", FT_STRING)
 
p_rnrp.fields = {f_nodenumber, f_path, f_area, f_counter}
 
-- myproto dissector function
function p_rnrp.dissector (buf, pkt, root)
  -- validate packet length is adequate, otherwise quit
  if buf:len() == 0 then return end
  pkt.cols.protocol = p_rnrp.name
 
  -- create subtree for myproto
  subtree = root:add(p_rnrp, buf(0))
  -- add protocol fields to subtree
  subtree:add(f_nodenumber, buf(4,2))
  subtree:add(f_area, buf(6,1))
  subtree:add(f_path, buf(7,1))
  subtree:add(f_counter, buf(74,2))
 
  -- description of payload
  subtree:append_text(", Command details here or in the tree below")
end
 
-- Initialization routine
function p_rnrp.init()
end
 
-- register a chained dissector for port 8002
local udp_dissector_table = DissectorTable.get("udp.port")
dissector = udp_dissector_table:get_dissector(2423)
  -- you can call dissector from function p_myproto.dissector above
  -- so that the previous dissector gets called
udp_dissector_table:add(2423, p_rnrp)